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
    private readonly List<Button> _quickSpecialButtons = [];
    private CombatEngine _combat = null!;
    private RichTextLabel _eventLog = null!;
    private ProgressBar _initiativeBar = null!;
    private Label _initiativeLabel = null!;
    private Label _clockLabel = null!;
    private Label _runStateLabel = null!;
    private Label _playerThreatLabel = null!;
    private Label _opponentThreatLabel = null!;
    private CheckButton _pauseButton = null!;
    private CheckButton _blockButton = null!;
    private CheckButton _aiAttackButton = null!;
    private CheckButton _aiDefenseButton = null!;
    private OptionButton _scriptSelector = null!;
    private int _eventIndex;
    private double _frameAccumulator;

    public override void _Ready()
    {
        _combat = new CombatEngine(_tuning);
        BuildInterface();
        GetWindow().MinSize = new Vector2I(1120, 760);
        UpdateInterface();
        AppendNewEvents();
    }

    public override void _Process(double delta)
    {
        if (!_pauseButton.ButtonPressed)
        {
            _frameAccumulator += Math.Min(delta, 0.20);
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

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is not InputEventKey key)
        {
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
                case Key.Period: StepOnce(); break;
                case Key.R: ResetRound(); break;
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
            OffsetTop = 14.0f,
            OffsetRight = -18.0f,
            OffsetBottom = -14.0f,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill
        };
        root.AddThemeConstantOverride("separation", 8);
        AddChild(root);

        var header = new HBoxContainer();
        header.AddChild(new Label
        {
            Text = "MELEE RHYTHM LAB · UI-ONLY",
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            VerticalAlignment = VerticalAlignment.Center
        });
        _clockLabel = new Label { CustomMinimumSize = new Vector2(106, 0), HorizontalAlignment = HorizontalAlignment.Right };
        header.AddChild(_clockLabel);
        _runStateLabel = new Label { Text = "RUNNING", CustomMinimumSize = new Vector2(86, 0), HorizontalAlignment = HorizontalAlignment.Center };
        header.AddChild(_runStateLabel);
        _pauseButton = new CheckButton { Text = "Pause [P]", CustomMinimumSize = new Vector2(108, 0), FocusMode = Control.FocusModeEnum.None };
        _pauseButton.Toggled += _ => UpdateInterface();
        header.AddChild(_pauseButton);
        header.AddChild(MakeButton("Step [.] ", StepOnce, 78));
        header.AddChild(MakeButton("Reset [R]", ResetRound, 88));
        root.AddChild(header);

        root.AddChild(new Label
        {
            Text = "Both fighters act continuously. Buffer attacks while defending; no turn handoff is imposed.",
            AutowrapMode = TextServer.AutowrapMode.WordSmart
        });

        var mainRow = new HBoxContainer { SizeFlagsVertical = Control.SizeFlags.ExpandFill };
        mainRow.AddThemeConstantOverride("separation", 10);
        root.AddChild(mainRow);
        var fightColumn = new VBoxContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill
        };
        fightColumn.AddThemeConstantOverride("separation", 7);
        mainRow.AddChild(fightColumn);
        mainRow.AddChild(BuildTuningPanel());

        var initiativePanel = MakePanel();
        var initiativeColumn = new VBoxContainer();
        initiativeColumn.AddChild(new Label { Text = "INITIATIVE · independent attack readiness" });
        _initiativeBar = new ProgressBar { MinValue = 0, MaxValue = 200, ShowPercentage = false, CustomMinimumSize = new Vector2(0, 12) };
        initiativeColumn.AddChild(_initiativeBar);
        _initiativeLabel = new Label { HorizontalAlignment = HorizontalAlignment.Center };
        initiativeColumn.AddChild(_initiativeLabel);
        initiativePanel.AddChild(initiativeColumn);
        fightColumn.AddChild(initiativePanel);

        var fighters = new HBoxContainer { SizeFlagsVertical = Control.SizeFlags.ShrinkBegin };
        fighters.AddThemeConstantOverride("separation", 8);
        fighters.AddChild(BuildFighterPanel("PLAYER", _playerUi));
        fighters.AddChild(BuildFighterPanel("SCRIPTED OPPONENT", _opponentUi));
        fightColumn.AddChild(fighters);
        fightColumn.AddChild(BuildAttackControls());
        fightColumn.AddChild(BuildDefenseControls());
        fightColumn.AddChild(BuildThreatChart("THREATS TO PLAYER", _combat.Opponent, _playerThreatTracks, out _playerThreatLabel));
        fightColumn.AddChild(BuildThreatChart("THREATS TO OPPONENT", _combat.Player, _opponentThreatTracks, out _opponentThreatLabel));

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
        logColumn.AddChild(_eventLog);
        logPanel.AddChild(logColumn);
        fightColumn.AddChild(logPanel);
    }

    private Control BuildFighterPanel(string title, FighterUi ui)
    {
        var panel = MakePanel();
        panel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        var column = new VBoxContainer();
        column.AddChild(new Label { Text = title });
        ui.HealthBar = new ProgressBar { MinValue = 0, MaxValue = 100, ShowPercentage = false, CustomMinimumSize = new Vector2(0, 10) };
        column.AddChild(ui.HealthBar);
        ui.HealthLabel = new Label();
        column.AddChild(ui.HealthLabel);
        ui.StaminaBar = new ProgressBar { MinValue = 0, MaxValue = 100, ShowPercentage = false, CustomMinimumSize = new Vector2(0, 12) };
        column.AddChild(ui.StaminaBar);
        ui.Reservation = new ColorRect
        {
            AnchorBottom = 1, MouseFilter = MouseFilterEnum.Ignore,
            Material = new ShaderMaterial { Shader = GD.Load<Shader>("res://staminaReservation.gdshader") }
        };
        ui.StaminaBar.AddChild(ui.Reservation);
        ui.StaminaLabel = new Label();
        column.AddChild(ui.StaminaLabel);
        ui.ActionLabel = new Label { AutowrapMode = TextServer.AutowrapMode.WordSmart, CustomMinimumSize = new Vector2(0, 32) };
        column.AddChild(ui.ActionLabel);
        ui.ActionBar = new ProgressBar { MinValue = 0, MaxValue = 100, ShowPercentage = false, CustomMinimumSize = new Vector2(0, 7) };
        column.AddChild(ui.ActionBar);
        ui.BufferLabel = new Label { AutowrapMode = TextServer.AutowrapMode.WordSmart, CustomMinimumSize = new Vector2(0, 36) };
        column.AddChild(ui.BufferLabel);
        panel.AddChild(column);
        return panel;
    }

    private Control BuildAttackControls()
    {
        var panel = MakePanel();
        var column = new VBoxContainer();
        column.AddChild(new Label
        {
            Text = "BUFFER ATTACKS · A/S/D = Light · Shift = Heavy · Alt/Ctrl = quick specials · " +
                "Shift+Alt/Ctrl = heavy specials"
        });
        var grid = new GridContainer { Columns = 4 };
        grid.AddChild(new Control { CustomMinimumSize = new Vector2(90, 0) });
        foreach (var lane in Lanes())
        {
            grid.AddChild(new Label
            {
                Text = lane.ToString().ToUpperInvariant(),
                HorizontalAlignment = HorizontalAlignment.Center,
                CustomMinimumSize = new Vector2(70, 0)
            });
        }

        AddAttackRow(grid, "Light", AttackKind.Light);
        AddAttackRow(grid, "Heavy", AttackKind.Heavy);
        AddAttackRow(grid, "Quick Unblockable", AttackKind.LightUnblockable);
        AddAttackRow(grid, "Quick Undodgeable", AttackKind.LightUndodgeable);
        AddAttackRow(grid, "Heavy Unblockable", AttackKind.Unblockable);
        AddAttackRow(grid, "Heavy Undodgeable", AttackKind.Undodgeable);
        column.AddChild(grid);
        panel.AddChild(column);
        return panel;
    }

    private void AddAttackRow(GridContainer grid, string title, AttackKind kind)
    {
        grid.AddChild(new Label { Text = title, VerticalAlignment = VerticalAlignment.Center });
        foreach (var lane in Lanes())
        {
            var button = new Button
            {
                Text = lane.ToString().ToUpperInvariant(),
                TooltipText = $"Queue a {lane} {title} immediately; execution waits until attack-ready.",
                CustomMinimumSize = new Vector2(70, 26),
                FocusMode = Control.FocusModeEnum.None,
                Disabled = (kind is AttackKind.LightUnblockable or AttackKind.LightUndodgeable) &&
                    !_tuning.UseExpandedSpecials
            };
            if (kind is AttackKind.LightUnblockable or AttackKind.LightUndodgeable)
            {
                _quickSpecialButtons.Add(button);
            }
            button.AddThemeStyleboxOverride("normal", CreateFlatStyle(GetAttackColor(kind).ToHtml(), "#405466", 1, 3));
            button.Pressed += () => QueuePlayerAttack(kind, lane);
            grid.AddChild(button);
        }
    }

    private Control BuildDefenseControls()
    {
        var panel = MakePanel();
        var column = new VBoxContainer();
        column.AddChild(new Label { Text = "DEFENSE · block can be held while attacks remain buffered" });
        var row = new HBoxContainer();
        _blockButton = new CheckButton { Text = "Hold block [Space]", FocusMode = Control.FocusModeEnum.None };
        _blockButton.Toggled += held => _combat.SetBlockHeld(_combat.Player, held);
        row.AddChild(_blockButton);
        foreach (var kind in new[] { DefenseKind.Parry, DefenseKind.Dodge, DefenseKind.Duck, DefenseKind.Hop })
        {
            row.AddChild(MakeButton($"{kind} [{GetDefenseKey(kind)}]", () => ActivatePlayerDefense(kind), 0));
        }

        column.AddChild(row);
        column.AddChild(new Label
        {
            Text = "Directional duck/hop are provisional. A/S/D lanes map High/Mid/Low; F/G/C/V activate the corresponding defense.",
            AutowrapMode = TextServer.AutowrapMode.WordSmart
        });
        panel.AddChild(column);
        return panel;
    }

    private Control BuildThreatChart(
        string title,
        CombatFighter attacker,
        LaneTrackUi[] tracks,
        out Label threatLabel)
    {
        var panel = MakePanel();
        var column = new VBoxContainer();
        var heading = new HBoxContainer();
        heading.AddChild(new Label { Text = title, SizeFlagsHorizontal = Control.SizeFlags.ExpandFill });
        threatLabel = new Label { HorizontalAlignment = HorizontalAlignment.Right };
        heading.AddChild(threatLabel);
        column.AddChild(heading);
        column.AddChild(new Label
        {
            Text = "Notes fall toward impact: Light white · Undodgeable blue · Unblockable orange. Green = counter window; pale = parry window.",
            AutowrapMode = TextServer.AutowrapMode.WordSmart
        });
        var laneRow = new HBoxContainer();
        laneRow.AddThemeConstantOverride("separation", 5);
        var keys = new[] { "A", "S", "D" };
        var lanes = Lanes();
        for (var index = 0; index < lanes.Length; index++)
        {
            var lane = lanes[index];
            var laneColumn = new VBoxContainer { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
            laneColumn.AddChild(new Label
            {
                Text = $"{lane.ToString().ToUpperInvariant()} · {keys[index]}",
                HorizontalAlignment = HorizontalAlignment.Center
            });
            var track = new PanelContainer
            {
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                CustomMinimumSize = new Vector2(0, 96),
                MouseFilter = Control.MouseFilterEnum.Ignore
            };
            track.AddThemeStyleboxOverride("panel", CreateFlatStyle("#101923", "#405466", 1, 3));
            var content = new Control { CustomMinimumSize = new Vector2(0, 96), MouseFilter = Control.MouseFilterEnum.Ignore };
            var goodBand = new ColorRect { Color = new Color("#5cbd8a", 0.26f), MouseFilter = Control.MouseFilterEnum.Ignore };
            var perfectBand = new ColorRect { Color = new Color("#b8f1cd", 0.46f), MouseFilter = Control.MouseFilterEnum.Ignore };
            var strikeLine = new ColorRect { Color = new Color("#e8edf2"), MouseFilter = Control.MouseFilterEnum.Ignore };
            var note = new PanelContainer { Visible = false, MouseFilter = Control.MouseFilterEnum.Ignore };
            note.AddChild(new Label { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
            content.AddChild(goodBand);
            content.AddChild(perfectBand);
            content.AddChild(strikeLine);
            content.AddChild(note);
            track.AddChild(content);
            laneColumn.AddChild(track);
            laneRow.AddChild(laneColumn);
            tracks[index] = new LaneTrackUi
            {
                Lane = lane,
                Attacker = attacker,
                Content = content,
                GoodBand = goodBand,
                PerfectBand = perfectBand,
                StrikeLine = strikeLine,
                Note = note,
                NoteLabel = (Label)note.GetChild(0)
            };
        }

        column.AddChild(laneRow);
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
        AddSlider(rows, "Light startup (s)", 0.1, 1.2, 0.001, () => _tuning.LightStartup, v => _tuning.LightStartup = v);
        AddSlider(rows, "Heavy startup (s)", 0.1, 1.5, 0.001, () => _tuning.HeavyStartup, v => _tuning.HeavyStartup = v);
        AddSlider(rows, "Quick special startup (s)", 0.1, 1.5, 0.001, () => _tuning.LightSpecialStartup, v => _tuning.LightSpecialStartup = v);
        AddSlider(rows, "Heavy special startup (s)", 0.1, 1.5, 0.001, () => _tuning.SpecialStartup, v => _tuning.SpecialStartup = v);
        AddSlider(rows, "Attack input interval (s)", 0, 0.2, 0.005, () => _tuning.AttackInputInterval, v => _tuning.AttackInputInterval = v);
        AddSlider(rows, "Telegraph lead (s)", 0, 1.2, 0.01, () => _tuning.TelegraphLead, v => _tuning.TelegraphLead = v);
        AddSlider(rows, "Parry window (s)", 0, 0.6, 0.01, () => _tuning.ParryWindow, v => _tuning.ParryWindow = v);
        AddSlider(rows, "Dodge window (s)", 0, 0.6, 0.01, () => _tuning.DodgeWindow, v => _tuning.DodgeWindow = v);
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
        AddSlider(rows, "AI attack cadence (s)", 0.2, 2.0, 0.01, () => _tuning.AiCadence, v => _tuning.AiCadence = v);
        AddSlider(rows, "AI initial delay (s)", 0, 2.0, 0.01, () => _tuning.AiInitialDelay, v => _tuning.AiInitialDelay = v);
        AddSlider(rows, "AI reaction delay (s)", 0, 0.8, 0.01, () => _tuning.AiReactionDelay, v => _tuning.AiReactionDelay = v);
        AddSlider(rows, "AI parry read (%)", 0, 100, 1, () => 100 * _tuning.AiParryReadChance, v => _tuning.AiParryReadChance = v / 100);

        panel.AddChild(column);
        return panel;
    }

    private void AddSlider(
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
        foreach (var (slider, getValue) in _tuningSliders)
        {
            slider.Value = getValue();
        }
        foreach (var button in _quickSpecialButtons)
        {
            button.Disabled = !useHybrid;
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
        _runStateLabel.Text = _pauseButton.ButtonPressed ? "PAUSED" : "RUNNING";
        _runStateLabel.AddThemeColorOverride("font_color",
            _pauseButton.ButtonPressed ? new Color("#f0c674") : new Color("#94d6a5"));
        _initiativeBar.Value = Math.Clamp(_combat.Initiative + 100.0, 0.0, 200.0);
        _initiativeLabel.Text = $"{_combat.InitiativeOwner} advantage · {_combat.Initiative:+0;-0;0} · " +
            $"last frame advantage {_combat.LastFrameAdvantage * 1000.0:+0;-0;0} ms";
        UpdateFighterUi(_combat.Player, _playerUi);
        UpdateFighterUi(_combat.Opponent, _opponentUi);
        UpdateThreatChart(_combat.Player, _playerThreatTracks, _playerThreatLabel);
        UpdateThreatChart(_combat.Opponent, _opponentThreatTracks, _opponentThreatLabel);
    }

    private void UpdateFighterUi(CombatFighter fighter, FighterUi ui)
    {
        ui.HealthBar.Value = fighter.Health;
        ui.HealthLabel.Text = $"HEALTH  {fighter.Health:0}/100";
        ui.StaminaBar.MaxValue = Math.Max(1.0, fighter.MaxStamina);
        ui.StaminaBar.Value = fighter.Stamina;
        ui.Reservation.AnchorLeft = (float)(fighter.AvailableStamina / ui.StaminaBar.MaxValue);
        ui.Reservation.AnchorRight = (float)(fighter.Stamina / ui.StaminaBar.MaxValue);
        ui.Reservation.Visible = fighter.ReservedStamina > 0;
        ui.StaminaLabel.Text = $"STAMINA  {fighter.AvailableStamina:0.#}/{fighter.MaxStamina:0.#}" +
            (fighter.ReservedStamina > 0 ? $"  ·  {fighter.ReservedStamina:0.#} reserved" : "");
        ui.StaminaBar.Modulate = Time.GetTicksMsec() < ui.FlashUntil ? new Color("#ff8070") : Colors.White;
        var attackReadyIn = Math.Max(0.0, fighter.AttackReadyAt - _combat.Time);
        if (fighter.CurrentAttack is { } attack)
        {
            ui.ActionLabel.Text = $"ATTACK STARTUP: {attack.Info.Lane} {attack.Info.Name} · " +
                $"{Math.Max(0.0, attack.Remaining):0.000}s until impact";
            ui.ActionBar.Value = attack.Progress * 100.0;
        }
        else
        {
            ui.ActionLabel.Text = attackReadyIn > 0.0
                ? $"ATTACK RECOVERY · ready in {attackReadyIn:0.000}s"
                : fighter.BufferedAttackCount > 0 ? "ATTACK QUEUED" : "ATTACK READY";
            ui.ActionBar.Value = attackReadyIn > 0.0 ? 0.0 : 100.0;
        }

        var queued = fighter.AttackBuffer.Count == 0
            ? "none"
            : string.Join(" → ", fighter.AttackBuffer.Select(FormatAttackName));
        ui.BufferLabel.Text = $"BUFFER ({fighter.BufferedAttackCount}/{CombatEngine.MaxBufferLength}): {queued}\n" +
            $"DEFENSE READY · block {ReadyIn(fighter.BlockReadyAt):0.00}s · parry {ReadyIn(fighter.ParryReadyAt):0.00}s · " +
            $"dodge {ReadyIn(fighter.DodgeReadyAt):0.00}s · duck {ReadyIn(fighter.DuckReadyAt):0.00}s · " +
            $"hop {ReadyIn(fighter.HopReadyAt):0.00}s · response: {fighter.LastDefenseResponse}";
    }

    private double ReadyIn(double readyAt) => Math.Max(0.0, readyAt - _combat.Time);

    private void UpdateThreatChart(CombatFighter defender, LaneTrackUi[] tracks, Label threatLabel)
    {
        var incoming = _combat.GetIncomingThreat(defender);
        threatLabel.Text = incoming is { } preview
            ? $"{preview.Info.Name} · impact in {Math.Max(0.0, preview.ImpactAt - _combat.Time):0.000}s"
            : "No visible threat";

        var activeWindow = Math.Max(_tuning.ParryWindow, _tuning.DodgeWindow);
        foreach (var track in tracks)
        {
            var contentSize = track.Content.Size;
            var width = Math.Max(0.0f, contentSize.X);
            var height = Math.Max(0.0f, contentSize.Y);
            if (width <= 0.0f || height <= 0.0f)
            {
                continue;
            }

            var trackThreat = incoming is { } threat && threat.Info.Lane == track.Lane ? threat : (AttackPreview?)null;
            var lineY = height - 19.0f;
            var noteHeight = 24.0f;
            var startY = 3.0f;
            var endY = Math.Max(startY, lineY - noteHeight);
            var travel = endY - startY;
            var visibleDuration = trackThreat is { } current
                ? Math.Max(0.001, current.ImpactAt - current.VisibleAt)
                : 1.0;
            var goodHeight = (float)(travel * Math.Clamp(activeWindow / visibleDuration, 0.0, 1.0));
            var perfectHeight = (float)(travel * Math.Clamp(_tuning.ParryWindow / visibleDuration, 0.0, 1.0));
            track.GoodBand.Position = new Vector2(0, endY - goodHeight);
            track.GoodBand.Size = new Vector2(width, goodHeight + noteHeight);
            track.PerfectBand.Position = new Vector2(0, endY - perfectHeight);
            track.PerfectBand.Size = new Vector2(width, perfectHeight + noteHeight);
            track.StrikeLine.Position = new Vector2(0, lineY);
            track.StrikeLine.Size = new Vector2(width, 2);
            track.Note.Visible = trackThreat is not null;
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

            var progress = Math.Clamp((_combat.Time - shown.VisibleAt) / visibleDuration, 0.0, 1.0);
            track.Note.Position = new Vector2(4, startY + travel * (float)progress);
            track.Note.Size = new Vector2(Math.Max(0.0f, width - 8), noteHeight);
        }
    }

    private void AppendNewEvents()
    {
        if (_combat is null || _eventLog is null)
        {
            return;
        }

        var startIndex = _eventIndex;
        while (_eventIndex < _combat.Events.Count)
        {
            AppendEvent(_combat.Events[_eventIndex++]);
        }

        if (_eventIndex - startIndex > 0 && _eventIndex > MaxVisibleEvents)
        {
            _eventLog.Clear();
            var firstVisibleEvent = Math.Max(0, _eventIndex - MaxVisibleEvents);
            for (var index = firstVisibleEvent; index < _eventIndex; index++)
            {
                AppendEvent(_combat.Events[index]);
            }
        }

        if (_eventLog.GetParagraphCount() > 0)
        {
            _eventLog.ScrollToLine(_eventLog.GetParagraphCount() - 1);
        }
    }

    private void AppendEvent(CombatEvent item)
    {
        var color = item.Kind switch
        {
            CombatEventKind.AttackResolved => "#f08c78",
            CombatEventKind.Defense => "#8de0c0",
            CombatEventKind.Stamina => "#89c8ef",
            CombatEventKind.AttackLaunched => "#d6c1f5",
            _ => "#c8d2df"
        };
        _eventLog.AppendText($"[color=#77879a][{item.Time,6:0.00}s][/color] " +
            $"[color={color}]{item.Message}[/color]\n");
    }

    private void QueuePlayerAttack(AttackKind kind, AttackLane lane)
    {
        if (!_combat.QueueAttack(
            _combat.Player,
            kind,
            lane,
            Time.GetTicksUsec() / 1_000_000.0) && _combat.Events[^1].Kind == CombatEventKind.InsufficientStamina)
            _playerUi.FlashUntil = Time.GetTicksMsec() + 180;
    }

    private void ActivatePlayerDefense(DefenseKind kind) =>
        _combat.ActivateDefense(_combat.Player, kind);

    private void StepOnce()
    {
        _combat.Tick(CombatEngine.FixedStep);
        _frameAccumulator = 0.0;
        UpdateInterface();
        AppendNewEvents();
    }

    private void ResetRound()
    {
        _combat = new CombatEngine(_tuning);
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
        AttackKind.LightUnblockable or AttackKind.Unblockable => new Color("#f39a42"),
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
        public ProgressBar HealthBar { get; set; } = null!;
        public Label HealthLabel { get; set; } = null!;
        public ProgressBar StaminaBar { get; set; } = null!;
        public ColorRect Reservation { get; set; } = null!;
        public ulong FlashUntil { get; set; }
        public Label StaminaLabel { get; set; } = null!;
        public Label ActionLabel { get; set; } = null!;
        public ProgressBar ActionBar { get; set; } = null!;
        public Label BufferLabel { get; set; } = null!;
    }

    private sealed class LaneTrackUi
    {
        public AttackLane Lane { get; init; }
        public CombatFighter Attacker { get; init; } = null!;
        public Control Content { get; init; } = null!;
        public ColorRect GoodBand { get; init; } = null!;
        public ColorRect PerfectBand { get; init; } = null!;
        public ColorRect StrikeLine { get; init; } = null!;
        public PanelContainer Note { get; init; } = null!;
        public Label NoteLabel { get; init; } = null!;
        public AttackKind? DrawnKind { get; set; }
    }
}
