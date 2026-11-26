using CombatPrototype.Core;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CombatPrototype;

public partial class Main : Control
{
    private const int MaxVisibleEvents = 280;
    private readonly CombatTuning _tuning = new();
    private readonly FighterUi _playerUi = new();
    private readonly FighterUi _opponentUi = new();
    private readonly LaneTrackUi[] _playerThreatTracks = new LaneTrackUi[3];
    private readonly LaneTrackUi[] _opponentThreatTracks = new LaneTrackUi[3];
    private readonly List<(HSlider Slider, Func<double> GetValue)> _tuningSliders = [];
    private OptionButton _attackSelector = null!;
    private Control _classicControls = null!;
    private Control _counterControls = null!;
    private Button _counterButton = null!;
    private bool _useCounter = true;
    private readonly HashSet<Key> _counterKeys = [];
    private readonly List<(LaneTrackUi Track, ColorRect Flash, ulong StartMs)> _outgoing = [];
    private const float PurchaseFlashMs = 140f;
    private CombatEngine _combat = null!;
    private RichTextLabel _eventLog = null!;
    private ProgressBar _initiativeBar = null!;
    private Label _initiativeLabel = null!;
    private Label _clockLabel = null!;
    private Label _runStateLabel = null!;
    private Button _speedButton = null!;
    private const double SlowSpeed = 0.605;
    private const string SlowSpeedLabel = "0.605x [T]";
    private bool _slowSpeed = true;
    private double PlaySpeed => _useCounter && _slowSpeed ? SlowSpeed : 1.0;
    private Label _playerThreatLabel = null!;
    private Label _opponentThreatLabel = null!;
    private CheckButton _pauseButton = null!;
    private CheckButton _blockButton = null!;
    private CheckButton _aiAttackButton = null!;
    private CheckButton _aiDefenseButton = null!;
    private OptionButton _scriptSelector = null!;
    private int _eventIndex;
    private double _frameAccumulator;
    private LivePlaytest? _livePlaytest;

    public override void _Ready()
    {
        var liveDirectory = System.Environment.GetEnvironmentVariable("COMBAT_LIVE_DIR");
        if (!string.IsNullOrWhiteSpace(liveDirectory)) _livePlaytest = new LivePlaytest(liveDirectory);
        _combat = CreateRound();
        BuildInterface();
        _combat.AttackQueued += ShowQueuedAttack;
        _combat.AttackInterrupted += ShowInterruptedNote;
        GetWindow().MinSize = new Vector2I(800, 500);
        GetWindow().FocusExited += ClearCounterInput;
        if (_livePlaytest is not null) _pauseButton.ButtonPressed = true;
        UpdateInterface();
        AppendNewEvents();
    }

    public override void _Process(double delta)
    {
        _livePlaytest?.Update(_combat, _pauseButton.ButtonPressed, PlaySpeed, GetWindow().HasFocus(), Time.GetTicksMsec());
        if (!_pauseButton.ButtonPressed)
        {
            _frameAccumulator += Math.Min(delta, 0.20) * PlaySpeed;
            var steps = 0;
            while (_frameAccumulator >= CombatEngine.FixedStep && steps < 8)
            {
                _combat.Tick(CombatEngine.FixedStep);
                _frameAccumulator -= CombatEngine.FixedStep;
                steps++;
            }

            if (steps == 8)
            {
                _frameAccumulator = Math.Min(_frameAccumulator, CombatEngine.FixedStep);
            }
        }

        UpdateInterface();
        AppendNewEvents();
    }

    public override void _ExitTree()
    {
        if (_combat is not null) _livePlaytest?.Complete(_combat);
    }

    public override void _Input(InputEvent @event)
    {
        if (_combat.CounterTrial is null || @event is not InputEventKey key ||
            (key.PhysicalKeycode != Key.O && key.Keycode != Key.O)) return;

        // Capture the combat tap before a focused UI control can consume it.
        if (key.Pressed && !key.Echo) AttackInCounter(fromKey: true);
        GetViewport().SetInputAsHandled();
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is not InputEventKey key)
        {
            return;
        }

        var actionKey = key.PhysicalKeycode != Key.None ? key.PhysicalKeycode : key.Keycode;
        if (key.Pressed && !key.Echo && (actionKey is Key.Escape or Key.Period or Key.R))
        {
            switch (actionKey)
            {
                case Key.Escape: _pauseButton.ButtonPressed = !_pauseButton.ButtonPressed; break;
                case Key.Period: StepOnce(); break;
                case Key.R: ResetRound(); break;
            }
            GetViewport().SetInputAsHandled();
            return;
        }

        if (_combat.CounterTrial is not null)
        {
            if (actionKey == Key.P)
            {
                if (!key.Echo)
                {
                    _parryKeyHeld = key.Pressed;
                    UpdateParryHold();
                }
            }
            else if (actionKey is Key.A or Key.D)
            {
                if (key.Pressed) _counterKeys.Add(actionKey);
                else _counterKeys.Remove(actionKey);
                ApplyCounterInput();
            }
            else if (key.Pressed && !key.Echo)
            {
                switch (actionKey)
                {
                    case Key.I: ActivatePlayerDefense(DefenseKind.Dodge); break;
                    case Key.T: TogglePlaySpeed(); break;
                    default: return;
                }
            }
            GetViewport().SetInputAsHandled();
            return;
        }

        if (key.Keycode == Key.Space)
        {
            if (_blockButton is not null)
            {
                _blockButton.ButtonPressed = key.Pressed;
            }

            _combat.SetBlockHeld(_combat.Player, key.Pressed);
            GetViewport().SetInputAsHandled();
            return;
        }

        if (!key.Pressed || key.Echo)
        {
            return;
        }

        if (TryGetLane(key.Keycode, out var lane))
        {
            var kind = key.AltPressed
                ? key.ShiftPressed ? AttackKind.Unblockable : AttackKind.LightUnblockable
                : key.CtrlPressed
                    ? key.ShiftPressed ? AttackKind.Undodgeable : AttackKind.LightUndodgeable
                    : key.ShiftPressed ? AttackKind.Heavy : AttackKind.Light;
            QueuePlayerAttack(kind, lane);
        }
        else
        {
            switch (key.Keycode)
            {
                case Key.F: ActivatePlayerDefense(DefenseKind.Parry); break;
                case Key.G: ActivatePlayerDefense(DefenseKind.Dodge); break;
                case Key.C: ActivatePlayerDefense(DefenseKind.Duck); break;
                case Key.V: ActivatePlayerDefense(DefenseKind.Hop); break;
                case Key.P: _pauseButton.ButtonPressed = !_pauseButton.ButtonPressed; break;
                default: return;
            }
        }

        GetViewport().SetInputAsHandled();
    }

    private void BuildInterface()
    {
        var root = new VBoxContainer
        {
            Name = "RootLayout",
            AnchorRight = 1.0f,
            AnchorBottom = 1.0f,
            OffsetLeft = 18.0f,
            OffsetTop = 9.0f,
            OffsetRight = -18.0f,
            OffsetBottom = -9.0f,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill
        };
        root.AddThemeConstantOverride("separation", 8);
        AddChild(root);

        var header = new HBoxContainer();
        var viewNav = new TabBar { ClipTabs = false, FocusMode = FocusModeEnum.None };
        foreach (var name in new[] { "Play", "Tuning", "Exchange" }) viewNav.AddTab(name);
        header.AddChild(viewNav);
        header.AddChild(new Control { SizeFlagsHorizontal = SizeFlags.ExpandFill });
        _clockLabel = new Label { Name = "CombatClock", CustomMinimumSize = new Vector2(106, 0), HorizontalAlignment = HorizontalAlignment.Right };
        header.AddChild(_clockLabel);
        _runStateLabel = new Label { Text = "RUNNING", CustomMinimumSize = new Vector2(86, 0), HorizontalAlignment = HorizontalAlignment.Center };
        header.AddChild(_runStateLabel);
        _pauseButton = new CheckButton { Text = "Pause [Esc]", CustomMinimumSize = new Vector2(108, 0), FocusMode = Control.FocusModeEnum.None };
        _pauseButton.Toggled += _ => UpdateInterface();
        header.AddChild(_pauseButton);
        header.AddChild(MakeButton("Step [.] ", StepOnce, 78));
        header.AddChild(MakeButton("Reset [R]", ResetRound, 88));
        root.AddChild(header);

        var tabs = new TabContainer { Name = "Views", TabsVisible = false, SizeFlagsVertical = SizeFlags.ExpandFill };
        root.AddChild(tabs);
        var play = new VBoxContainer { Name = "Play" };
        tabs.AddChild(play);
        play.AddChild(BuildSpacingDisplay());
        var charts = new HBoxContainer { Name = "Charts", SizeFlagsVertical = SizeFlags.ExpandFill };
        charts.AddThemeConstantOverride("separation", 12);
        play.AddChild(charts);
        charts.AddChild(BuildThreatChart("PLAYER", _playerUi, _playerThreatTracks, out _playerThreatLabel));
        charts.AddChild(BuildThreatChart("OPPONENT AI", _opponentUi, _opponentThreatTracks, out _opponentThreatLabel));
        var modeRow = new HBoxContainer();
        play.AddChild(modeRow);
        var mode = new OptionButton { Name = "TrialMode", FocusMode = FocusModeEnum.None };
        mode.AddItem("Counter trial");
        mode.AddItem("Classic charts");
        mode.ItemSelected += index =>
        {
            _useCounter = index == 0;
            RefreshTuningSliders();
            ResetRound();
        };
        modeRow.AddChild(mode);
        _speedButton = MakeButton(SlowSpeedLabel, TogglePlaySpeed, 84);
        _speedButton.Name = "PlaySpeed";
        _speedButton.TooltipText = "Toggle slow speed or full speed";
        modeRow.AddChild(_speedButton);
        _classicControls = BuildCombatControls();
        modeRow.AddChild(_classicControls);
        _counterControls = BuildCounterControls();
        modeRow.AddChild(_counterControls);
        var tuning = BuildTuningPanel();
        tuning.Name = "Tuning";
        tabs.AddChild(tuning);
        var telemetry = new VBoxContainer { Name = "Exchange" };
        tabs.AddChild(telemetry);

        var initiativePanel = MakePanel();
        var initiativeColumn = new VBoxContainer();
        initiativeColumn.AddChild(new Label { Text = "INITIATIVE · independent attack readiness" });
        _initiativeBar = new ProgressBar { MinValue = 0, MaxValue = 200, ShowPercentage = false, CustomMinimumSize = new Vector2(0, 12) };
        initiativeColumn.AddChild(_initiativeBar);
        _initiativeLabel = new Label { HorizontalAlignment = HorizontalAlignment.Center };
        initiativeColumn.AddChild(_initiativeLabel);
        initiativePanel.AddChild(initiativeColumn);
        telemetry.AddChild(initiativePanel);
        foreach (var ui in new[] { _playerUi, _opponentUi })
        {
            ui.DetailLabel = new Label { AutowrapMode = TextServer.AutowrapMode.WordSmart };
            telemetry.AddChild(ui.DetailLabel);
        }

        var logPanel = MakePanel();
        logPanel.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
        var logColumn = new VBoxContainer { SizeFlagsVertical = Control.SizeFlags.ExpandFill };
        logColumn.AddChild(new Label { Text = "EXCHANGE LOG · outcomes, resource changes, readiness and frame advantage" });
        _eventLog = new RichTextLabel
        {
            BbcodeEnabled = true,
            ScrollFollowing = true,
            FitContent = false,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill,
            CustomMinimumSize = new Vector2(0, 130),
            SelectionEnabled = true
        };
        _eventLog.VisibilityChanged += AppendNewEvents;
        logColumn.AddChild(_eventLog);
        logPanel.AddChild(logColumn);
        telemetry.AddChild(logPanel);
        viewNav.TabChanged += index => tabs.CurrentTab = (int)index;
        tabs.TabChanged += index => viewNav.CurrentTab = (int)index;
    }

    private Control BuildVitals(FighterUi ui)
    {
        var row = new HBoxContainer();
        row.AddThemeConstantOverride("separation", 12);
        ui.DamageLabel = SmallLabel();
        ui.DamageLabel.Name = "DamageTaken";
        ui.DamageLabel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        ui.DamageLabel.AddThemeColorOverride("font_color", new Color("#efb29f"));
        row.AddChild(ui.DamageLabel);
        var stamina = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        row.AddChild(stamina);
        ui.StaminaLabel = SmallLabel();
        stamina.AddChild(ui.StaminaLabel);
        ui.StaminaBar = new ProgressBar { MaxValue = 100, ShowPercentage = false, CustomMinimumSize = new Vector2(0, 7) };
        ui.StaminaBar.AddThemeStyleboxOverride("fill", new StyleBoxFlat { BgColor = new Color("#7ad7b3") });
        stamina.AddChild(ui.StaminaBar);
        ui.Reservation = new ColorRect
        {
            AnchorBottom = 1, MouseFilter = MouseFilterEnum.Ignore,
            Material = new ShaderMaterial { Shader = GD.Load<Shader>("res://staminaReservation.gdshader") }
        };
        ui.StaminaBar.AddChild(ui.Reservation);
        return row;
    }

    private static Label SmallLabel()
    {
        var label = new Label { ClipText = true, TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis };
        label.AddThemeFontSizeOverride("font_size", 13);
        return label;
    }

    private Control BuildCombatControls()
    {
        var toolbar = new HFlowContainer { Name = "CombatControls", SizeFlagsHorizontal = SizeFlags.ExpandFill };
        toolbar.AddThemeConstantOverride("h_separation", 16);
        var attacks = new HBoxContainer();
        toolbar.AddChild(attacks);
        _attackSelector = new OptionButton { FocusMode = FocusModeEnum.None, TooltipText = "Attack type for the lane buttons" };
        foreach (var kind in new[] { AttackKind.Light, AttackKind.Heavy, AttackKind.Unblockable,
            AttackKind.Undodgeable, AttackKind.LightUnblockable, AttackKind.LightUndodgeable })
        {
            _attackSelector.AddItem(_combat.DescribeAttack(kind).Name, (int)kind);
        }
        UpdateSpecialOptions();
        attacks.AddChild(_attackSelector);
        foreach (var lane in Lanes())
            attacks.AddChild(MakeButton(lane.ToString(), () => QueuePlayerAttack((AttackKind)_attackSelector.GetSelectedId(), lane), 0));

        var defenses = new HBoxContainer();
        toolbar.AddChild(defenses);
        _blockButton = new CheckButton { Text = "Block", TooltipText = "Hold block (Space)", FocusMode = FocusModeEnum.None };
        _blockButton.Toggled += held => _combat.SetBlockHeld(_combat.Player, held);
        defenses.AddChild(_blockButton);
        foreach (var kind in new[] { DefenseKind.Parry, DefenseKind.Dodge, DefenseKind.Duck, DefenseKind.Hop })
        {
            var button = MakeButton(kind.ToString(), () => ActivatePlayerDefense(kind), 0);
            button.TooltipText = $"{kind} ({GetDefenseKey(kind)})";
            defenses.AddChild(button);
        }
        return toolbar;
    }

    private void UpdateSpecialOptions()
    {
        foreach (var kind in new[] { AttackKind.LightUnblockable, AttackKind.LightUndodgeable })
        {
            var index = _attackSelector.GetItemIndex((int)kind);
            _attackSelector.SetItemDisabled(index, !_tuning.UseExpandedSpecials);
            if (!_tuning.UseExpandedSpecials && _attackSelector.Selected == index) _attackSelector.Select(0);
        }
    }

    private Control BuildThreatChart(
        string title,
        FighterUi ui,
        LaneTrackUi[] tracks,
        out Label threatLabel)
    {
        var panel = MakePanel();
        panel.Name = title == "PLAYER" ? "PlayerChart" : "AiChart";
        panel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        var column = new VBoxContainer();
        var heading = new HBoxContainer();
        var owner = new Label { Text = title };
        owner.AddThemeColorOverride("font_color", new Color(title == "PLAYER" ? "#85dcb9" : "#f4aa82"));
        heading.AddChild(owner);
        threatLabel = SmallLabel();
        threatLabel.HorizontalAlignment = HorizontalAlignment.Right;
        threatLabel.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        heading.AddChild(threatLabel);
        column.AddChild(heading);
        column.AddChild(BuildVitals(ui));
        ui.IncomingQueue = BuildQueueRail(title == "PLAYER");
        column.AddChild(ui.IncomingQueue.Row);
        var laneRow = new HBoxContainer { SizeFlagsVertical = SizeFlags.ExpandFill };
        laneRow.AddThemeConstantOverride("separation", 5);
        var keys = new[] { "A", "S", "D" };
        var lanes = Lanes();
        for (var index = 0; index < lanes.Length; index++)
        {
            var lane = lanes[index];
            var laneColumn = new VBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
            var headingLabel = new Label
            {
                Text = title == "PLAYER" ? $"{lane.ToString().ToUpperInvariant()} · {keys[index]}" : lane.ToString().ToUpperInvariant(),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            laneColumn.AddChild(headingLabel);
            var track = new PanelContainer
            {
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                SizeFlagsVertical = SizeFlags.ExpandFill,
                CustomMinimumSize = new Vector2(0, 96),
                MouseFilter = Control.MouseFilterEnum.Ignore
            };
            track.AddThemeStyleboxOverride("panel", CreateFlatStyle("#101923", "#405466", 1, 3));
            var content = new Control { Name = "LaneSurface", ClipContents = true, MouseFilter = Control.MouseFilterEnum.Ignore };
            var goodBand = new ColorRect { Color = new Color("#5cbd8a", 0.26f), MouseFilter = Control.MouseFilterEnum.Ignore };
            var perfectBand = new ColorRect { Color = new Color("#b8f1cd", 0.46f), MouseFilter = Control.MouseFilterEnum.Ignore };
            var strikeLine = new ColorRect { Color = new Color("#e8edf2"), MouseFilter = Control.MouseFilterEnum.Ignore };
            var note = new PanelContainer { Name = "IncomingNote", Visible = false, MouseFilter = Control.MouseFilterEnum.Ignore };
            var caption = SmallLabel();
            caption.HorizontalAlignment = HorizontalAlignment.Center;
            note.AddChild(caption);
            var burstNotes = new Control { Name = "BurstNotes", Visible = false, MouseFilter = Control.MouseFilterEnum.Ignore };
            content.AddChild(goodBand);
            content.AddChild(perfectBand);
            content.AddChild(strikeLine);
            content.AddChild(burstNotes);
            burstNotes.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
            content.AddChild(note);
            track.AddChild(content);
            laneColumn.AddChild(track);
            laneRow.AddChild(laneColumn);
            tracks[index] = new LaneTrackUi
            {
                Lane = lane,
                Column = laneColumn,
                Heading = headingLabel,
                Content = content,
                GoodBand = goodBand,
                PerfectBand = perfectBand,
                StrikeLine = strikeLine,
                Note = note,
                NoteLabel = (Label)note.GetChild(0),
                BurstNotes = burstNotes
            };
            var laneTrack = tracks[index];
            burstNotes.Draw += () => DrawBurstNotes(laneTrack, title == "PLAYER" ? _combat.Opponent : _combat.Player);
        }

        column.AddChild(laneRow);
        ui.ActionLabel = SmallLabel();
        ui.BufferLabel = SmallLabel();
        column.AddChild(ui.ActionLabel);
        column.AddChild(ui.BufferLabel);
        panel.AddChild(column);
        return panel;
    }

    private Control BuildTuningPanel()
    {
        var panel = MakePanel();
        panel.CustomMinimumSize = new Vector2(330, 0);
        panel.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
        var column = new VBoxContainer();
        column.AddChild(new Label { Text = "EXPERIMENTAL TUNING" });
        column.AddChild(new Label
        {
            Text = "Baseline values are provisional. Choose the hybrid experiment to compare a Sifu/OSU timing pass.",
            AutowrapMode = TextServer.AutowrapMode.WordSmart
        });

        var timingPreset = new OptionButton();
        timingPreset.AddItem("Baseline timing");
        timingPreset.AddItem("Hybrid timing experiment");
        timingPreset.ItemSelected += index =>
        {
            ApplyTimingPreset(index == 1);
            ResetRound();
        };
        column.AddChild(timingPreset);

        _scriptSelector = new OptionButton();
        _scriptSelector.AddItem("Mixed sequence");
        _scriptSelector.AddItem("Pressure sequence");
        _scriptSelector.AddItem("Mix-up sequence");
        _scriptSelector.ItemSelected += index =>
        {
            _tuning.AiScript = index switch { 1 => "Pressure", 2 => "Mix-up", _ => "Mixed" };
        };
        column.AddChild(_scriptSelector);
        _aiAttackButton = new CheckButton { Text = "Scripted opponent attacks", ButtonPressed = _tuning.AiAttackEnabled };
        _aiAttackButton.Toggled += enabled => _tuning.AiAttackEnabled = enabled;
        column.AddChild(_aiAttackButton);
        _aiDefenseButton = new CheckButton { Text = "Opponent defensive reactions", ButtonPressed = _tuning.AiDefenseEnabled };
        _aiDefenseButton.Toggled += enabled => _tuning.AiDefenseEnabled = enabled;
        column.AddChild(_aiDefenseButton);

        var scroll = new ScrollContainer
        {
            SizeFlagsVertical = Control.SizeFlags.ExpandFill,
            HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled
        };
        var rows = new VBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        scroll.AddChild(rows);
        column.AddChild(scroll);

        AddSlider(rows, "Starting stamina", 0, 100, 1, () => _tuning.StartingStamina, v => _tuning.StartingStamina = v);
        AddSlider(rows, "Maximum stamina", 1, 150, 1, () => _tuning.MaxStamina, v => _tuning.MaxStamina = v);
        AddSlider(rows, "Stamina regeneration / sec", 0, 50, 1, () => _tuning.StaminaRegenerationPerSecond, v => _tuning.StaminaRegenerationPerSecond = v);
        AddSlider(rows, "Light stamina cost", 0, 100, 1, () => _tuning.LightCost, v => _tuning.LightCost = v);
        AddSlider(rows, "Heavy stamina cost", 0, 100, 1, () => _tuning.HeavyCost, v => _tuning.HeavyCost = v);
        AddSlider(rows, "Quick special cost", 0, 100, 1, () => _tuning.LightSpecialCost, v => _tuning.LightSpecialCost = v);
        AddSlider(rows, "Heavy special cost", 0, 100, 1, () => _tuning.SpecialCost, v => _tuning.SpecialCost = v);
        AddSlider(rows, "Light startup (s)", 0.1, 1.2, 0.001,
            () => _useCounter ? _tuning.CounterTrialLightStartup : _tuning.LightStartup, v =>
            {
                if (_useCounter) _tuning.CounterTrialLightStartup = v;
                else _tuning.LightStartup = v;
            }).Name = "LightStartup";
        AddSlider(rows, "Heavy startup (s)", 0.1, 1.5, 0.001, () => _tuning.HeavyStartup, v => _tuning.HeavyStartup = v);
        AddSlider(rows, "Quick special startup (s)", 0.1, 1.5, 0.001, () => _tuning.LightSpecialStartup, v => _tuning.LightSpecialStartup = v);
        AddSlider(rows, "Heavy special startup (s)", 0.1, 1.5, 0.001, () => _tuning.SpecialStartup, v => _tuning.SpecialStartup = v);
        AddSlider(rows, "Attack input interval (s)", 0, 0.2, 0.005, () => _tuning.AttackInputInterval, v => _tuning.AttackInputInterval = v);
        AddSlider(rows, "Telegraph lead (s)", 0, 1.2, 0.01, () => _tuning.TelegraphLead, v => _tuning.TelegraphLead = v);
        AddSlider(rows, "Parry / Counter trial window (s)", 0, 0.6, 0.01, () => _tuning.ParryWindow, v => _tuning.ParryWindow = v);
        AddSlider(rows, "Dodge window (classic, s)", 0, 0.6, 0.01, () => _tuning.DodgeWindow, v => _tuning.DodgeWindow = v);
        AddSlider(rows, "Directional counter window (s)", 0, 0.6, 0.01, () => _tuning.DirectionalCounterWindow, v => _tuning.DirectionalCounterWindow = v);

        AddSlider(rows, "Light hit: attacker +", 0, 50, 1, () => _tuning.LightAttackerCleanHitGain, v => _tuning.LightAttackerCleanHitGain = v);
        AddSlider(rows, "Light hit: defender +", 0, 50, 1, () => _tuning.LightDefenderCleanHitGain, v => _tuning.LightDefenderCleanHitGain = v);
        AddSlider(rows, "Light block: attacker +", 0, 50, 1, () => _tuning.LightAttackerBlockedGain, v => _tuning.LightAttackerBlockedGain = v);
        AddSlider(rows, "Light block: defender +", 0, 50, 1, () => _tuning.LightDefenderBlockedGain, v => _tuning.LightDefenderBlockedGain = v);
        AddSlider(rows, "Light dodge: attacker +", 0, 50, 1, () => _tuning.LightAttackerDodgedGain, v => _tuning.LightAttackerDodgedGain = v);
        AddSlider(rows, "Light dodge: defender +", 0, 50, 1, () => _tuning.LightDefenderDodgedGain, v => _tuning.LightDefenderDodgedGain = v);
        AddSlider(rows, "Light parry: attacker +", 0, 50, 1, () => _tuning.LightAttackerParriedGain, v => _tuning.LightAttackerParriedGain = v);
        AddSlider(rows, "Light parry: defender +", 0, 50, 1, () => _tuning.LightDefenderParriedGain, v => _tuning.LightDefenderParriedGain = v);
        AddSlider(rows, "Heavy hit: attacker +", 0, 50, 1, () => _tuning.HeavyAttackerCleanHitGain, v => _tuning.HeavyAttackerCleanHitGain = v);
        AddSlider(rows, "Heavy hit: defender +", 0, 50, 1, () => _tuning.HeavyDefenderCleanHitGain, v => _tuning.HeavyDefenderCleanHitGain = v);
        AddSlider(rows, "Heavy block: attacker +", 0, 50, 1, () => _tuning.HeavyAttackerBlockedGain, v => _tuning.HeavyAttackerBlockedGain = v);
        AddSlider(rows, "Heavy block: defender +", 0, 50, 1, () => _tuning.HeavyDefenderBlockedGain, v => _tuning.HeavyDefenderBlockedGain = v);
        AddSlider(rows, "Heavy dodge: attacker +", 0, 50, 1, () => _tuning.HeavyAttackerDodgedGain, v => _tuning.HeavyAttackerDodgedGain = v);
        AddSlider(rows, "Heavy dodge: defender +", 0, 50, 1, () => _tuning.HeavyDefenderDodgedGain, v => _tuning.HeavyDefenderDodgedGain = v);
        AddSlider(rows, "Heavy parry: attacker +", 0, 50, 1, () => _tuning.HeavyAttackerParriedGain, v => _tuning.HeavyAttackerParriedGain = v);
        AddSlider(rows, "Heavy parry: defender +", 0, 50, 1, () => _tuning.HeavyDefenderParriedGain, v => _tuning.HeavyDefenderParriedGain = v);
        AddSlider(rows, "Special counter gain", 0, 50, 1, () => _tuning.SpecialCounterGain, v => _tuning.SpecialCounterGain = v);

        AddSlider(rows, "Light blocked attacker recovery", 0, 1.2, 0.01, () => _tuning.LightBlockedAttackerRecovery, v => _tuning.LightBlockedAttackerRecovery = v);
        AddSlider(rows, "Light blocked defender recovery", 0, 1.2, 0.01, () => _tuning.LightBlockedDefenderRecovery, v => _tuning.LightBlockedDefenderRecovery = v);
        AddSlider(rows, "Heavy blocked attacker recovery", 0, 1.2, 0.01, () => _tuning.HeavyBlockedAttackerRecovery, v => _tuning.HeavyBlockedAttackerRecovery = v);
        AddSlider(rows, "Heavy blocked defender recovery", 0, 1.2, 0.01, () => _tuning.HeavyBlockedDefenderRecovery, v => _tuning.HeavyBlockedDefenderRecovery = v);
        AddSlider(rows, "Dodged attacker recovery", 0, 1.2, 0.01, () => _tuning.DodgedAttackerRecovery, v => _tuning.DodgedAttackerRecovery = v);
        AddSlider(rows, "Dodged defender recovery", 0, 1.2, 0.01, () => _tuning.DodgedDefenderRecovery, v => _tuning.DodgedDefenderRecovery = v);
        AddSlider(rows, "Parried attacker recovery", 0, 1.2, 0.01, () => _tuning.ParriedAttackerRecovery, v => _tuning.ParriedAttackerRecovery = v);
        AddSlider(rows, "Parried defender recovery", 0, 1.2, 0.01, () => _tuning.ParriedDefenderRecovery, v => _tuning.ParriedDefenderRecovery = v);
        AddSlider(rows, "Hit attacker recovery", 0, 1.2, 0.01, () => _tuning.HitAttackerRecovery, v => _tuning.HitAttackerRecovery = v);
        AddSlider(rows, "Hit defender recovery", 0, 1.2, 0.01, () => _tuning.HitDefenderRecovery, v => _tuning.HitDefenderRecovery = v);
        AddSlider(rows, "Special attacker recovery (provisional)", 0, 1.2, 0.01, () => _tuning.SpecialAttackerRecovery, v => _tuning.SpecialAttackerRecovery = v);
        AddSlider(rows, "Special counter recovery", 0, 1.2, 0.01, () => _tuning.SpecialCounterAttackerRecovery, v => _tuning.SpecialCounterAttackerRecovery = v);
        AddSlider(rows, "Special defender recovery (provisional)", 0, 1.2, 0.01, () => _tuning.SpecialDefenderRecovery, v => _tuning.SpecialDefenderRecovery = v);
        AddSlider(rows, "Block action recovery", 0, 1.2, 0.01, () => _tuning.BlockActionRecovery, v => _tuning.BlockActionRecovery = v);
        AddSlider(rows, "Parry action recovery", 0, 1.2, 0.01, () => _tuning.ParryActionRecovery, v => _tuning.ParryActionRecovery = v);
        AddSlider(rows, "Dodge action recovery", 0, 1.2, 0.01, () => _tuning.DodgeActionRecovery, v => _tuning.DodgeActionRecovery = v);
        AddSlider(rows, "Duck action recovery", 0, 1.2, 0.01, () => _tuning.DuckActionRecovery, v => _tuning.DuckActionRecovery = v);
        AddSlider(rows, "Hop action recovery", 0, 1.2, 0.01, () => _tuning.HopActionRecovery, v => _tuning.HopActionRecovery = v);
        AddSlider(rows, "AI attack cadence (s)", 0.2, 2.0, 0.01, () => _tuning.AiCadence, v => _tuning.AiCadence = v).Name = "AiCadence";
        AddSlider(rows, "AI initial delay (s)", 0, 2.0, 0.01, () => _tuning.AiInitialDelay, v => _tuning.AiInitialDelay = v);
        AddSlider(rows, "AI reaction delay (s)", 0, 0.8, 0.01, () => _tuning.AiReactionDelay, v => _tuning.AiReactionDelay = v);
        AddSlider(rows, "AI parry read (%)", 0, 100, 1, () => 100 * _tuning.AiParryReadChance, v => _tuning.AiParryReadChance = v / 100);

        panel.AddChild(column);
        return panel;
    }

    private HSlider AddSlider(
        VBoxContainer parent,
        string title,
        double minimum,
        double maximum,
        double step,
        Func<double> getValue,
        Action<double> setValue)
    {
        var row = new VBoxContainer();
        row.AddThemeConstantOverride("separation", 1);
        var heading = new HBoxContainer();
        heading.AddChild(new Label { Text = title, SizeFlagsHorizontal = Control.SizeFlags.ExpandFill });
        var valueLabel = new Label { CustomMinimumSize = new Vector2(52, 0), HorizontalAlignment = HorizontalAlignment.Right };
        heading.AddChild(valueLabel);
        row.AddChild(heading);
        var slider = new HSlider
        {
            MinValue = minimum,
            MaxValue = maximum,
            Step = step,
            Value = Math.Clamp(getValue(), minimum, maximum),
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            FocusMode = Control.FocusModeEnum.None
        };
        _tuningSliders.Add((slider, getValue));
        valueLabel.Text = FormatTuningValue(slider.Value);
        slider.ValueChanged += value =>
        {
            setValue(value);
            valueLabel.Text = FormatTuningValue(value);
        };
        row.AddChild(slider);
        parent.AddChild(row);
        return slider;
    }

    private void ApplyTimingPreset(bool useHybrid)
    {
        _tuning.UseExpandedSpecials = useHybrid;
        _tuning.LightSpecialCost = 20.0;
        _tuning.SpecialCost = useHybrid ? 35.0 : 30.0;
        _tuning.LightSpecialStartup = 0.500;
        _tuning.SpecialStartup = useHybrid ? 0.800 : 0.700;
        _tuning.LightUnblockableDamage = 16.0;
        _tuning.LightUndodgeableDamage = 16.0;
        _tuning.UnblockableDamage = 30.0;
        _tuning.UndodgeableDamage = useHybrid ? 24.0 : 20.0;
        _tuning.DodgedDefenderRecovery = useHybrid ? 0.175 : 0.100;
        RefreshTuningSliders();
        UpdateSpecialOptions();
    }

    private void RefreshTuningSliders()
    {
        foreach (var (slider, getValue) in _tuningSliders)
        {
            slider.Value = getValue();
        }
    }

    private static string FormatTuningValue(double value) => value < 10.0
        ? value.ToString("0.000")
        : value.ToString("0.#");

    private void UpdateInterface()
    {
        if (_combat is null || _pauseButton is null)
        {
            return;
        }

        _clockLabel.Text = $"TIME  {_combat.Time:00.00}s";
        _speedButton.Visible = _useCounter;
        _speedButton.Text = _slowSpeed ? SlowSpeedLabel : "1x [T]";
        _runStateLabel.Text = _pauseButton.ButtonPressed ? "PAUSED" : "RUNNING";
        _runStateLabel.AddThemeColorOverride("font_color",
            _pauseButton.ButtonPressed ? new Color("#f0c674") : new Color("#94d6a5"));
        if (_initiativeBar.IsVisibleInTree())
        {
            _initiativeBar.Value = Math.Clamp(_combat.Initiative + 100.0, 0.0, 200.0);
            _initiativeLabel.Text = $"{_combat.InitiativeOwner} advantage · {_combat.Initiative:+0;-0;0} · " +
                $"last frame advantage {_combat.LastFrameAdvantage * 1000.0:+0;-0;0} ms";
        }
        UpdateFighterUi(_combat.Player, _playerUi);
        UpdateFighterUi(_combat.Opponent, _opponentUi);
        UpdateThreatChart(_combat.Player, _playerThreatTracks, _playerThreatLabel);
        UpdateThreatChart(_combat.Opponent, _opponentThreatTracks, _opponentThreatLabel);
        UpdateCounterUi();
        UpdateOutgoing();
    }

    private void UpdateFighterUi(CombatFighter fighter, FighterUi ui)
    {
        UpdateQueueRail(ui, fighter.IsPlayer ? _combat.Opponent : _combat.Player);
        ui.DamageLabel.Text = $"DAMAGE TAKEN  {fighter.DamageTaken:0.#}";
        ui.StaminaBar.MaxValue = Math.Max(1.0, fighter.MaxStamina);
        ui.StaminaBar.Value = fighter.Stamina;
        ui.Reservation.AnchorLeft = (float)(fighter.AvailableStamina / ui.StaminaBar.MaxValue);
        ui.Reservation.AnchorRight = (float)(fighter.Stamina / ui.StaminaBar.MaxValue);
        ui.Reservation.Visible = fighter.ReservedStamina > 0;
        ui.StaminaLabel.Text = $"STAMINA  {fighter.AvailableStamina:0.#}/{fighter.MaxStamina:0.#}" +
            (fighter.ReservedStamina > 0 ? $"  (+{fighter.ReservedStamina:0.#} held)" : "");
        ui.StaminaBar.Modulate = Time.GetTicksMsec() < ui.FlashUntil ? new Color("#ff8070") : Colors.White;
        var attackReadyIn = Math.Max(0.0, _combat.GetAttackLaunchReadyAt(fighter) - _combat.Time);
        if (fighter.CurrentAttack is { } attack)
        {
            ui.ActionLabel.Text = $"ATTACK STARTUP: {attack.Info.Lane} {attack.Info.Name} · " +
                $"{Math.Max(0.0, attack.Remaining):0.000}s until impact";
        }
        else if (fighter.BufferedAttackCount > 0 && _combat.CounterTrial?.IsParryHeld(fighter.IsPlayer) == true)
        {
            ui.ActionLabel.Text = "QUEUED · release P to attack";
        }
        else
        {
            ui.ActionLabel.Text = attackReadyIn > 0.0
                ? $"ATTACK RECOVERY · ready in {attackReadyIn:0.000}s"
                : fighter.BufferedAttackCount > 0 ? "ATTACK QUEUED" : "ATTACK READY";
        }

        const int previewCount = 4;
        var count = _combat.WaitingAttackCount(fighter);
        var preview = string.Join(" → ", fighter.PreviewAttackBuffer(previewCount).Select(FormatAttackName));
        if (fighter.BufferedAttackCount > previewCount) preview += $" +{fighter.BufferedAttackCount - previewCount}";
        var queued = count > fighter.BufferedAttackCount
            ? (preview.Length == 0 ? "Follow-up" : $"{preview} + follow-up")
            : (preview.Length == 0 ? "none" : preview);
        var (queueState, queueSize) = _combat.AttackQueueLimit(fighter) is { } max
            ? (count >= max ? "QUEUE FULL" : "QUEUE", $"{count}/{max}")
            : ("QUEUE", $"{count}");
        if (count == 0 && fighter.LastQueueClearedAt is { } clearedAt && _combat.Time - clearedAt < 0.30)
            queueState = "QUEUE CLEARED";
        ui.BufferLabel.Text = $"{queueState}  {queueSize} · {queued}";
        if (ui.DetailLabel.IsVisibleInTree())
        {
            ui.DetailLabel.Text = $"{fighter.Name} · {ui.BufferLabel.Text}\n" +
                $"DEFENSE READY · block {ReadyIn(fighter.BlockReadyAt):0.00}s · parry {ReadyIn(fighter.ParryReadyAt):0.00}s · " +
                $"dodge {ReadyIn(fighter.DodgeReadyAt):0.00}s · duck {ReadyIn(fighter.DuckReadyAt):0.00}s · " +
                $"hop {ReadyIn(fighter.HopReadyAt):0.00}s · response: {fighter.LastDefenseResponse}";
        }
    }

    private double ReadyIn(double readyAt) => Math.Max(0.0, readyAt - _combat.Time);

    private void ShowQueuedAttack(CombatFighter owner, AttackInfo info)
    {
        var track = (owner.IsPlayer ? _playerThreatTracks : _opponentThreatTracks)[(int)info.Lane];
        var flash = new ColorRect
        {
            Color = GetAttackColor(info.Kind), Modulate = new Color(1, 1, 1, 0.5f),
            MouseFilter = MouseFilterEnum.Ignore, ZIndex = 1
        };
        track.Content.AddChild(flash);
        _outgoing.Add((track, flash, Time.GetTicksMsec()));
        UpdateOutgoing();
    }

    private void UpdateOutgoing()
    {
        var now = Time.GetTicksMsec();
        for (var i = _outgoing.Count - 1; i >= 0; i--)
        {
            var (track, flash, startMs) = _outgoing[i];
            var t = (now - startMs) / PurchaseFlashMs;
            if (t >= 1f)
            {
                flash.QueueFree();
                _outgoing.RemoveAt(i);
                continue;
            }
            var size = track.Content.Size;
            flash.Size = new Vector2(size.X * 0.64f, 7);
            flash.Position = new Vector2(size.X * 0.18f,
                StrikeY(size.Y) - flash.Size.Y - Mathf.Min(size.Y * 0.4f, 150f) * t);
            flash.Modulate = new Color(1, 1, 1, 0.5f * (1f - t));
        }
    }

    private static float StrikeY(float height) => height * 0.87f;

    private void ShowInterruptedNote(AttackState attack)
    {
        var track = (attack.Defender.IsPlayer ? _playerThreatTracks : _opponentThreatTracks)[(int)attack.Info.Lane];
        ClearInterruptedNote(track);
        var ghost = new PanelContainer { Name = "InterruptedNote", MouseFilter = MouseFilterEnum.Ignore };
        ghost.AddThemeStyleboxOverride("panel", CreateFlatStyle("#413b3c", "#edaa96", 1, 3));
        var label = SmallLabel();
        label.Text = "INTERRUPTED";
        label.HorizontalAlignment = HorizontalAlignment.Center;
        label.MouseFilter = MouseFilterEnum.Ignore;
        ghost.AddChild(label);
        track.Content.AddChild(ghost);
        track.InterruptedNote = ghost;

        const float noteHeight = 24f;
        var progress = Math.Clamp(attack.Progress, 0, 1);
        const float startY = 3f;
        var travel = Math.Max(0, StrikeY(track.Content.Size.Y) - noteHeight - startY);
        ghost.Position = track.Note.Visible && track.DrawnKind == attack.Info.Kind
            ? track.Note.Position : new Vector2(4, startY + travel * (float)progress);
        ghost.Size = new Vector2(Math.Max(0, track.Content.Size.X - 8), noteHeight);
        var fade = ghost.CreateTween().SetParallel();
        fade.TweenProperty(ghost, "modulate:a", 0.0, 0.26);
        fade.TweenProperty(ghost, "position:y", Math.Max(3, ghost.Position.Y - 9), 0.26);
        fade.Chain().TweenCallback(Callable.From(() =>
        {
            if (ReferenceEquals(track.InterruptedNote, ghost)) ClearInterruptedNote(track);
        }));
    }

    private static void ClearInterruptedNote(LaneTrackUi track)
    {
        if (track.InterruptedNote is null) return;
        track.InterruptedNote.QueueFree();
        track.InterruptedNote = null;
    }

    private void UpdateThreatChart(CombatFighter defender, LaneTrackUi[] tracks, Label threatLabel)
    {
        var attacker = defender.IsPlayer ? _combat.Opponent : _combat.Player;
        var prediction = _combat.GetIncomingThreat(defender);
        var incoming = prediction is { Launched: true } ? prediction : null;
        threatLabel.Text = incoming is { } preview
            ? $"{preview.Info.Name} · impact in {Math.Max(0.0, preview.ImpactAt - _combat.Time):0.000}s"
            : _tuning.TelegraphLead > 0 && prediction is { } expected ? $"Anticipated: {expected.Info.Name}" : "No visible threat";

        var activeWindow = Math.Max(_tuning.ParryWindow, _combat.EffectiveDodgeWindow);
        foreach (var track in tracks)
        {
            if (!track.Column.Visible) continue;
            var contentSize = track.Content.Size;
            var width = Math.Max(0.0f, contentSize.X);
            var height = Math.Max(0.0f, contentSize.Y);
            if (width <= 0.0f || height <= 0.0f)
            {
                continue;
            }

            var trackThreat = incoming is { } threat && threat.Info.Lane == track.Lane ? threat : (AttackPreview?)null;
            var lineY = StrikeY(height);
            var noteHeight = 24.0f;
            const float startY = 3f;
            var endY = Math.Max(startY, lineY - noteHeight);
            var travel = endY - startY;
            var visibleDuration = trackThreat is { } current
                ? Math.Max(0.001, current.ImpactAt - current.VisibleAt)
                : 1.0;
            var goodHeight = (float)(travel * Math.Clamp(activeWindow * 0.5 / visibleDuration, 0.0, 1.0));
            var perfectHeight = (float)(travel * Math.Clamp(_tuning.ParryWindow * 0.5 / visibleDuration, 0.0, 1.0));
            track.GoodBand.Position = new Vector2(0, lineY - goodHeight);
            track.GoodBand.Size = new Vector2(width, goodHeight * 2);
            track.PerfectBand.Position = new Vector2(0, lineY - perfectHeight);
            track.PerfectBand.Size = new Vector2(width, perfectHeight * 2);
            track.GoodBand.Visible = track.PerfectBand.Visible = !_useCounter || trackThreat is not null;
            track.StrikeLine.Position = new Vector2(0, lineY);
            track.StrikeLine.Size = new Vector2(width, 2);
            track.BurstNotes.Visible = trackThreat is not null && attacker.ActiveAttacks.Count > 1;
            if (track.BurstNotes.Visible) track.BurstNotes.QueueRedraw();
            track.Note.Visible = trackThreat is not null && !track.BurstNotes.Visible;
            if (trackThreat is not { } shown)
            {
                continue;
            }

            if (track.DrawnKind != shown.Info.Kind)
            {
                var color = GetAttackColor(shown.Info.Kind);
                track.Note.AddThemeStyleboxOverride("panel", CreateFlatStyle(color.ToHtml(), "#f4f6f8", 1, 3));
                track.NoteLabel.Text = shown.Info.Name.ToUpperInvariant();
                track.NoteLabel.AddThemeColorOverride("font_color",
                    shown.Info.Kind is AttackKind.LightUndodgeable or AttackKind.Undodgeable
                        ? new Color("#ffffff")
                        : new Color("#18212a"));
                track.DrawnKind = shown.Info.Kind;
            }

            var progress = Math.Max(0.0, (_combat.Time - shown.VisibleAt) / visibleDuration);
            track.Note.Position = new Vector2(4, startY + travel * (float)progress);
            track.Note.Size = new Vector2(Math.Max(0.0f, width - 8), noteHeight);
        }
    }

    private void AppendNewEvents()
    {
        if (_combat is null || _eventLog is null || !_eventLog.IsVisibleInTree())
        {
            return;
        }

        var events = _combat.Events;
        if (_eventIndex == events.Count) return;
        var first = Math.Max(0, events.Count - MaxVisibleEvents);
        if (first >= _eventIndex)
        {
            _eventLog.Clear();
            _eventIndex = first;
        }
        else
        {
            var previousFirst = Math.Max(0, _eventIndex - MaxVisibleEvents);
            for (var index = previousFirst; index < first; index++)
            {
                // Each event has its own paragraph and closes all BBCode tags.
                _eventLog.RemoveParagraph(0, noInvalidate: true);
            }
        }
        while (_eventIndex < events.Count)
        {
            AppendEvent(events[_eventIndex++]);
        }
    }

    private void AppendEvent(CombatEvent item)
    {
        var color = item.Kind switch
        {
            CombatEventKind.AttackResolved or CombatEventKind.AttackQueueCleared => "#f08c78",
            CombatEventKind.Defense => "#8de0c0",
            CombatEventKind.Stamina => "#89c8ef",
            CombatEventKind.AttackLaunched => "#d6c1f5",
            _ => "#c8d2df"
        };
        _eventLog.AppendText($"[color=#77879a][{item.Time,6:0.00}s][/color] " +
            $"[color={color}]{item.Message}[/color]\n");
    }

    private bool QueuePlayerAttack(AttackKind kind, AttackLane lane)
    {
        var accepted = _combat.QueueAttack(
            _combat.Player,
            kind,
            lane,
            Time.GetTicksUsec() / 1_000_000.0);
        if (!accepted && _combat.Events[^1].Kind == CombatEventKind.InsufficientStamina)
            _playerUi.FlashUntil = Time.GetTicksMsec() + 180;
        return accepted;
    }

    private void ActivatePlayerDefense(DefenseKind kind)
    {
        _combat.ActivateDefense(_combat.Player, kind);
    }

    private void StepOnce()
    {
        _combat.Tick(CombatEngine.FixedStep);
        _frameAccumulator = 0.0;
        UpdateInterface();
        AppendNewEvents();
    }

    private void TogglePlaySpeed()
    {
        _slowSpeed = !_slowSpeed;
        UpdateInterface();
    }

    private void ResetRound()
    {
        _combat.AttackQueued -= ShowQueuedAttack;
        _combat.AttackInterrupted -= ShowInterruptedNote;
        foreach (var entry in _outgoing) entry.Flash.QueueFree();
        _outgoing.Clear();
        foreach (var track in _playerThreatTracks.Concat(_opponentThreatTracks)) ClearInterruptedNote(track);
        _combat = CreateRound();
        ClearCounterInput();
        _combat.AttackQueued += ShowQueuedAttack;
        _combat.AttackInterrupted += ShowInterruptedNote;
        _eventIndex = 0;
        _frameAccumulator = 0.0;
        _eventLog.Clear();
        _pauseButton.ButtonPressed = false;
        _blockButton.ButtonPressed = false;
        UpdateInterface();
        AppendNewEvents();
    }

    private Button MakeButton(string title, Action action, float minimumWidth)
    {
        var button = new Button
        {
            Text = title,
            CustomMinimumSize = new Vector2(minimumWidth, 0),
            FocusMode = Control.FocusModeEnum.None
        };
        button.Pressed += action;
        return button;
    }

    private static PanelContainer MakePanel()
    {
        var panel = new PanelContainer();
        panel.AddThemeStyleboxOverride("panel", CreateFlatStyle("#151e2a", "#334454", 1, 4));
        return panel;
    }

    private static StyleBoxFlat CreateFlatStyle(string background, string border, int borderWidth, int radius) =>
        new()
        {
            BgColor = new Color(background),
            BorderColor = new Color(border),
            BorderWidthLeft = borderWidth,
            BorderWidthTop = borderWidth,
            BorderWidthRight = borderWidth,
            BorderWidthBottom = borderWidth,
            CornerRadiusTopLeft = radius,
            CornerRadiusTopRight = radius,
            CornerRadiusBottomLeft = radius,
            CornerRadiusBottomRight = radius,
            ContentMarginLeft = 4,
            ContentMarginTop = 3,
            ContentMarginRight = 4,
            ContentMarginBottom = 3
        };

    private static AttackLane[] Lanes() => [AttackLane.High, AttackLane.Mid, AttackLane.Low];

    private static bool TryGetLane(Key key, out AttackLane lane)
    {
        if (key == Key.A) lane = AttackLane.High;
        else if (key == Key.S) lane = AttackLane.Mid;
        else if (key == Key.D) lane = AttackLane.Low;
        else
        {
            lane = AttackLane.High;
            return false;
        }

        return true;
    }

    private static string GetDefenseKey(DefenseKind kind) => kind switch
    {
        DefenseKind.Parry => "F",
        DefenseKind.Dodge => "G",
        DefenseKind.Duck => "C",
        DefenseKind.Hop => "V",
        _ => "Space"
    };

    private static Color GetAttackColor(AttackKind kind) => kind switch
    {
        AttackKind.LightUndodgeable or AttackKind.Undodgeable => new Color("#3187e8"),
        AttackKind.LightUnblockable or AttackKind.Unblockable or AttackKind.Unparryable => new Color("#f39a42"),
        _ => new Color("#f3f5f7")
    };

    private static string FormatAttackName(AttackIntent intent) =>
        $"{intent.Lane} {intent.Kind switch
        {
            AttackKind.LightUnblockable => "Quick Unblockable",
            AttackKind.LightUndodgeable => "Quick Undodgeable",
            AttackKind.Unblockable => "Heavy Unblockable",
            AttackKind.Undodgeable => "Heavy Undodgeable",
            _ => intent.Kind.ToString()
        }}";

    private sealed class FighterUi
    {
        public (PanelContainer Row, Label Count, Label Next) IncomingQueue { get; set; }
        public Label DamageLabel { get; set; } = null!;
        public ProgressBar StaminaBar { get; set; } = null!;
        public ColorRect Reservation { get; set; } = null!;
        public ulong FlashUntil { get; set; }
        public Label StaminaLabel { get; set; } = null!;
        public Label ActionLabel { get; set; } = null!;
        public Label DetailLabel { get; set; } = null!;
        public Label BufferLabel { get; set; } = null!;
    }

    private sealed class LaneTrackUi
    {
        public AttackLane Lane { get; init; }
        public Control Column { get; init; } = null!;
        public Label Heading { get; init; } = null!;
        public Control Content { get; init; } = null!;
        public ColorRect GoodBand { get; init; } = null!;
        public ColorRect PerfectBand { get; init; } = null!;
        public ColorRect StrikeLine { get; init; } = null!;
        public PanelContainer Note { get; init; } = null!;
        public Label NoteLabel { get; init; } = null!;
        public Control BurstNotes { get; init; } = null!;
        public AttackKind? DrawnKind { get; set; }
        public PanelContainer? InterruptedNote { get; set; }
    }
}
