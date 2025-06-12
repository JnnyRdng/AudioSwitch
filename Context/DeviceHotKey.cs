namespace AudioSwitch.Context;

public record DeviceHotKey(string DeviceName, uint Modifiers, Keys Key, int Id);