namespace BabylonArchiveCore.Core.Input;

public sealed class InputMap
{
    private readonly Dictionary<InputAction, List<InputBinding>> _bindings = new();

    public static InputMap CreateDefault()
    {
        var map = new InputMap();

        map.Bind(InputAction.MoveUp, new InputBinding(InputDeviceType.Keyboard, "W"));
        map.Bind(InputAction.MoveDown, new InputBinding(InputDeviceType.Keyboard, "S"));
        map.Bind(InputAction.MoveLeft, new InputBinding(InputDeviceType.Keyboard, "A"));
        map.Bind(InputAction.MoveRight, new InputBinding(InputDeviceType.Keyboard, "D"));
        map.Bind(InputAction.Interact, new InputBinding(InputDeviceType.Keyboard, "E"));
        map.Bind(InputAction.Confirm, new InputBinding(InputDeviceType.Keyboard, "Enter"));
        map.Bind(InputAction.Cancel, new InputBinding(InputDeviceType.Keyboard, "Escape"));
        map.Bind(InputAction.OpenArchive, new InputBinding(InputDeviceType.Keyboard, "Tab"));
        map.Bind(InputAction.CameraPan, new InputBinding(InputDeviceType.Mouse, "MiddleDrag"));
        map.Bind(InputAction.CameraZoom, new InputBinding(InputDeviceType.Mouse, "Wheel"));
        map.Bind(InputAction.CameraToggle, new InputBinding(InputDeviceType.Keyboard, "Q"));
        map.Bind(InputAction.Interact, new InputBinding(InputDeviceType.Touch, "Tap"));
        map.Bind(InputAction.Confirm, new InputBinding(InputDeviceType.Touch, "TapHold"));
        map.Bind(InputAction.CameraPan, new InputBinding(InputDeviceType.Touch, "Drag"));
        map.Bind(InputAction.CameraZoom, new InputBinding(InputDeviceType.Touch, "Pinch"));

        return map;
    }

    public IReadOnlyList<InputBinding> GetBindings(InputAction action)
    {
        return _bindings.TryGetValue(action, out var bindings)
            ? bindings
            : [];
    }

    public void Bind(InputAction action, InputBinding binding)
    {
        ArgumentNullException.ThrowIfNull(binding);

        if (!_bindings.TryGetValue(action, out var bindings))
        {
            bindings = [];
            _bindings[action] = bindings;
        }

        bindings.Add(binding);
    }

    public bool Matches(InputAction action, InputBinding candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        return _bindings.TryGetValue(action, out var bindings)
            && bindings.Contains(candidate);
    }
}
