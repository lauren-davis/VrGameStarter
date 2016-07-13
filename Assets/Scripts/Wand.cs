using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Valve.VR;

/// <summary>
/// Enumeration of SteamVR controller buttons.
/// </summary>
public enum WandButton
{
    Menu = EVRButtonId.k_EButton_ApplicationMenu,
    Grip = EVRButtonId.k_EButton_Grip,
    Touchpad = EVRButtonId.k_EButton_SteamVR_Touchpad,
    Trigger = EVRButtonId.k_EButton_SteamVR_Trigger
}

/// <summary>
/// Helper class for reading information about a SteamVR tracked controller.
/// </summary>
public sealed class Wand : MonoBehaviour
{
    /// <summary>
    /// Helper class for reading information about a SteamVR controller button.
    /// </summary>
    public sealed class Button
    {
        private readonly Wand _wand;
        private readonly WandButton _buttonId;

        private bool _wasPressed;

        internal Button(Wand wand, WandButton buttonId)
        {
            _wand = wand;
            _buttonId = buttonId;
        }

        /// <summary>
        /// If true, the button is currently being held down.
        /// </summary>
        public bool IsPressed
        {
            get
            {
                var controller = _wand.Controller;
                if (controller == null) return false;
                return controller.GetPressDown((EVRButtonId) _buttonId);
            }
        }
        
        /// <summary>
        /// If true, the button has just gone from being released to being pressed.
        /// </summary>
        public bool JustPressed { get { return !_wasPressed && IsPressed; } }

        /// <summary>
        /// If true, the button has just gone from being pressed to being released.
        /// </summary>
        public bool JustReleased { get { return _wasPressed && !IsPressed; } }

        internal void LateUpdate()
        {
            _wasPressed = IsPressed;
        }
    }

    private SteamVR_TrackedObject _trackedObject;
    private readonly Dictionary<WandButton, Button> _buttons; 

    private SteamVR_Controller.Device Controller { get { return SteamVR_Controller.Input((int) _trackedObject.index); } }

    public Wand()
    {
        _buttons = new Dictionary<WandButton, Button>();

        foreach (var buttonId in Enum.GetValues(typeof (WandButton)).Cast<WandButton>())
        {
            _buttons.Add(buttonId, new Button(this, buttonId));
        }
    }

    /// <summary>
    /// Holds the application menu button state.
    /// </summary>
    public Button MenuButton { get { return GetButton(WandButton.Menu); } }

    /// <summary>
    /// Holds the side grip buttons state.
    /// </summary>
    public Button GripButton { get { return GetButton(WandButton.Grip); } }

    /// <summary>
    /// Holds the large touchpad button state.
    /// </summary>
    public Button TouchpadButton { get { return GetButton(WandButton.Touchpad); } }

    /// <summary>
    /// Holds the trigger button state.
    /// </summary>
    public Button TriggerButton { get { return GetButton(WandButton.Trigger); } }

    /// <summary>
    /// Returns a number between 0.0 and 1.0 representing how pressed the
    /// trigger button is.
    /// </summary>
    public float TriggerValue
    {
        get
        {
            var controller = Controller;
            if (controller == null) return 0f;
            return controller.hairTriggerDelta;
        }
    }

    /// <summary>
    /// Causes the touchpad to vibrate for the given duration in microseconds.
    /// A good range of values is 250 for small pulses, and 1000 for large ones.
    /// </summary>
    /// <param name="durationMicroSecs">Duration of the pulse in microseconds.</param>
    public void HapticPulse(int durationMicroSecs = 500)
    {
        var controller = Controller;
        if (controller == null) return;
        controller.TriggerHapticPulse((ushort) durationMicroSecs, EVRButtonId.k_EButton_Grip);
    }

    /// <summary>
    /// Gets the state of a given button.
    /// </summary>
    /// <param name="button">Button to get the state of.</param>
    public Button GetButton(WandButton button)
    {
        return _buttons[button];
    }

    [UsedImplicitly]
    private void Awake()
    {
        _trackedObject = GetComponent<SteamVR_TrackedObject>();
    }

    [UsedImplicitly]
    private void LateUpdate()
    {
        foreach (var button in _buttons.Values)
        {
            button.LateUpdate();
        }
    }
}
