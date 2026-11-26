using System;
using CombatPrototype.Core;
using Godot;

namespace CombatPrototype;

public partial class LatencyDuel
{
    private static readonly Color Mint = new("#82ddbc");
    private static readonly Color Orange = new("#ffa373");
    private static readonly Color Ink = new("#101820");
    private static readonly Color Metal = new("#334653");
    private LineEdit _address = null!;
    private SpinBox _port = null!;
    private Button _hostButton = null!;
    private Button _joinButton = null!;
    private Button _leaveButton = null!;
    private Button _startButton = null!;
    private Button _resetButton = null!;
    private Label _connection = null!;
    private Label _network = null!;
    private Label _running = null!;
    private Control _arena = null!;
    private SideUi _self = null!;
    private SideUi _remote = null!;

    private sealed record SideUi(Label Damage, Label Queue, Label Result, ColorRect Lane);

    private void BuildDuelInterface()
    {
        var root = new VBoxContainer();
        AddChild(root);
        root.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        root.OffsetLeft = 16;
        root.OffsetRight = -16;
        root.OffsetTop = 12;
        root.OffsetBottom = -12;
        root.AddThemeConstantOverride("separation", 8);

        var connectionRow = new HBoxContainer();
        root.AddChild(connectionRow);
        connectionRow.AddChild(new Label { Text = "IPv4" });
        _address = new LineEdit
        {
            Name = "DuelAddress", Text = "127.0.0.1", MaxLength = 15,
            CustomMinimumSize = new Vector2(150, 0), TooltipText = "Host bind address or the host's address"
        };
        connectionRow.AddChild(_address);
        connectionRow.AddChild(new Label { Text = "Port" });
        _port = new SpinBox
        {
            Name = "DuelPort", MinValue = 1024, MaxValue = 65535, Step = 1, Value = DefaultPort
        };
        connectionRow.AddChild(_port);
        _hostButton = AddButton(connectionRow, "DuelHost", "Host", () => Connect(true));
        _joinButton = AddButton(connectionRow, "DuelJoin", "Join", () => Connect(false));
        _leaveButton = AddButton(connectionRow, "DuelLeave", "Disconnect", () => Disconnect());

        var sessionRow = new HBoxContainer();
        root.AddChild(sessionRow);
        _connection = new Label { Name = "DuelConnection", SizeFlagsHorizontal = SizeFlags.ExpandFill };
        sessionRow.AddChild(_connection);
        _network = new Label
        {
            Name = "DuelNetwork", Text = "RTT --", HorizontalAlignment = HorizontalAlignment.Right,
            CustomMinimumSize = new Vector2(170, 0)
        };
        sessionRow.AddChild(_network);
        _running = new Label { Name = "DuelClock", CustomMinimumSize = new Vector2(130, 0) };
        sessionRow.AddChild(_running);
        _startButton = AddButton(sessionRow, "DuelStart", "Play", ToggleDuel);
        _resetButton = AddButton(sessionRow, "DuelReset", "Reset", ResetDuel);

        _arena = new ColorRect { Color = new Color("#162521"), CustomMinimumSize = new Vector2(0, 58) };
        root.AddChild(_arena);
        _arena.Draw += DrawArena;
        var charts = new HBoxContainer { SizeFlagsVertical = SizeFlags.ExpandFill };
        charts.AddThemeConstantOverride("separation", 10);
        root.AddChild(charts);
        _self = AddSide(charts, true);
        _remote = AddSide(charts, false);
    }

    private static Button AddButton(HBoxContainer row, string name, string text, System.Action pressed)
    {
        var button = new Button { Name = name, Text = text, FocusMode = FocusModeEnum.None };
        button.Pressed += pressed;
        row.AddChild(button);
        return button;
    }

    private SideUi AddSide(HBoxContainer row, bool self)
    {
        var column = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        row.AddChild(column);
        var title = new Label { Text = self ? "YOU" : "OPPONENT" };
        title.AddThemeColorOverride("font_color", self ? Mint : Orange);
        column.AddChild(title);
        var damage = new Label { Name = self ? "DuelYourDamage" : "DuelTheirDamage" };
        var queue = new Label { Name = self ? "DuelYourQueue" : "DuelTheirQueue" };
        var lane = new ColorRect
        {
            Color = Ink, CustomMinimumSize = new Vector2(0, 155),
            SizeFlagsVertical = SizeFlags.ExpandFill, MouseFilter = MouseFilterEnum.Ignore
        };
        lane.Draw += () => DrawLane(lane, self);
        var result = new Label { Name = self ? "DuelYourResult" : "DuelTheirResult" };
        column.AddChild(damage);
        column.AddChild(queue);
        column.AddChild(lane);
        column.AddChild(result);
        return new SideUi(damage, queue, result, lane);
    }

    private void UpdateDuelInterface()
    {
        _connection.Text = _status;
        _network.Text = _rtt is { } rtt ? $"RTT {rtt:0} ms  /  VAR {_rttVariation:0} ms" : "RTT --";
        _running.Text = _frame is { } frame
            ? $"{frame.Clock:0.00}s  {(frame.Paused ? "PAUSED" : "LIVE")}" : "0.605x";
        _hostButton.Disabled = _joinButton.Disabled = _peer is not null;
        _leaveButton.Disabled = _peer is null;
        _address.Editable = _port.Editable = _peer is null;
        _startButton.Disabled = _resetButton.Disabled = !_hosting || !_connected;
        _startButton.Text = _paused ? "Play" : "Pause";
        var self = _hosting ? _frame?.Host : _frame?.Guest;
        var remote = _hosting ? _frame?.Guest : _frame?.Host;
        UpdateSide(_self, self);
        UpdateSide(_remote, remote);
        _arena.QueueRedraw();

        void UpdateSide(SideUi ui, Side? side)
        {
            ui.Damage.Text = side is null ? "DAMAGE --  /  STAMINA --" : $"DAMAGE {side.Damage:0}  /  STAMINA {side.Stamina:0}";
            ui.Queue.Text = side is null ? "QUEUE --" :
                $"QUEUE {side.Waiting}/{_frame!.QueueLimit}  /  {side.Next?.ToUpperInvariant() ?? "EMPTY"}";
            ui.Result.Text = side is null ? "-" : side.Guard ? "BLOCKING" :
                side.Counter is { } counter ? $"READY: {counter.ToUpperInvariant()}" :
                string.IsNullOrEmpty(side.Result) ? "-" : side.Result;
            ui.Lane.QueueRedraw();
        }
    }

    private void DrawArena()
    {
        var width = _arena.Size.X;
        _arena.DrawLine(new Vector2(0, 44), new Vector2(width, 44), Metal);
        if (_frame is not { } frame) return;
        var self = _hosting ? frame.Host : frame.Guest;
        var remote = _hosting ? frame.Guest : frame.Host;
        DrawFighter(self, Mint);
        DrawFighter(remote, Orange);

        void DrawFighter(Side side, Color color)
        {
            var x = (float)(_hosting ? side.Position : 1 - side.Position) * width;
            var bodyWidth = Math.Max(4, (float)CounterTrial.BodyGap * width);
            _arena.DrawRect(new Rect2(x - bodyWidth / 2, 12, bodyWidth, 32), color, filled: false);
            _arena.DrawCircle(new Vector2(x, 22), 4, color);
            _arena.DrawLine(new Vector2(x, 28), new Vector2(x, 42), color, 5);
        }
    }

    private void DrawLane(ColorRect lane, bool self)
    {
        var size = lane.Size;
        if (size.X <= 0 || size.Y <= 0) return;
        var line = size.Y * 0.87f;
        lane.DrawRect(new Rect2(Vector2.Zero, size), Metal, filled: false);
        var side = _hosting == self ? _frame?.Guest : _frame?.Host;
        if (_frame is { } frame && side is { Notes.Length: > 0 })
        {
            const float noteHeight = 18;
            const float start = 4;
            var travel = Math.Max(0, line - noteHeight - start);
            var band = travel * (float)(frame.Window / (side.Notes[0].Impact - side.Notes[0].Launch) / 2);
            lane.DrawRect(new Rect2(1, line - band, size.X - 2, Math.Min(2 * band, size.Y - line + band)),
                new Color("#315b4a"));
            foreach (var note in side.Notes)
            {
                var progress = (float)Math.Max(0, (frame.Clock - note.Launch) / (note.Impact - note.Launch));
                var y = start + travel * progress;
                if (y >= size.Y) continue;
                var color = note.Kind switch
                {
                    AttackKind.Unparryable => new Color("#f8a049"),
                    AttackKind.Undodgeable => new Color("#3988db"),
                    _ => new Color("#f0f3f7")
                };
                lane.DrawRect(new Rect2(4, y, size.X - 8, noteHeight), color);
                lane.DrawString(ThemeDB.FallbackFont, new Vector2(4, y + 13), note.Kind.ToString().ToUpperInvariant(),
                    HorizontalAlignment.Center, size.X - 8, 11, note.Kind == AttackKind.Undodgeable ? Colors.White : Ink);
            }
        }
        lane.DrawLine(new Vector2(4, line), new Vector2(size.X - 4, line), new Color("#dce7eb"), 2);
    }
}
