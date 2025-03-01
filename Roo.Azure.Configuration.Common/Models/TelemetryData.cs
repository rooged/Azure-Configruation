namespace Roo.Azure.Configuration.Common.Models
{
    /// <summary>
    /// Additional data that can be sent with telemetry logs.
    /// </summary>
    public class TelemetryData
    {
        /// <summary>
        /// User information of who's accessing the applciation.
        /// </summary>
        public User? User { get; set; }
        /// <summary>
        /// User session information.
        /// </summary>
        public Session? Session { get; set; }
        /// <summary>
        /// User device information.<br/>
        /// Most data is hidden by browsers so most information is unuasble in a web context.
        /// </summary>
        public Device? Device { get; set; }
        /// <summary>
        /// Component information of what the application is running on.
        /// </summary>
        public Component? Component { get; set; }
        /// <summary>
        /// Cloud information of the instance the application is running on.
        /// </summary>
        public Cloud? Cloud { get; set; }
        /// <summary>
        /// Operation information, i.e. transaction-id.
        /// </summary>
        public Operation? Operation { get; set; }
        /// <summary>
        /// Location information of where the application is being accessed from.
        /// </summary>
        public Location? Location { get; set; }
    }

    /// <summary>
    /// User information of who's accessing the applciation.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique Id is automatically genered in default App Insights configuration.
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// Unique Id of an application-defined account associated with the user.
        /// </summary>
        public string? AccountId { get; set; }
        /// <summary>
        /// Agent of an application-defined account associated with the user.
        /// </summary>
        public string? UserAgent { get; set; }
        /// <summary>
        /// Unique Id of an authenticated application-defined account associated with the user.<br/>
        /// Should be a persistant and unique to each authenticated user in the application or service.
        /// </summary>
        public string? AuthenticatedUserId { get; set; }
    }

    /// <summary>
    /// Information on the current session in the applciation.
    /// </summary>
    public class Session
    {
        /// <summary>
        /// Id of the current session.
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// Whether it's the first time a user is accessing a resource.
        /// </summary>
        public bool? IsFirst { get; set; }
    }

    /// <summary>
    /// Device information from the user accessing the applciation.
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Unique Id for the device.
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// The type of device.
        /// </summary>
        public string? Type { get; set; }
        /// <summary>
        /// Operating system for the current device.
        /// </summary>
        public string? OperatingSystem { get; set; }
        /// <summary>
        /// OEM name of the current device.
        /// </summary>
        public string? OemName { get; set; }
        /// <summary>
        /// Model of the current device.
        /// </summary>
        public string? Model { get; set; }
    }

    /// <summary>
    /// Component information of what the applciation is running on.
    /// </summary>
    public class Component
    {
        /// <summary>
        /// Application version.
        /// </summary>
        public string? Version { get; set; }
    }

    /// <summary>
    /// Cloud information of what the applciation is running on.
    /// </summary>
    public class Cloud
    {
        /// <summary>
        /// Cloud role name of what the application is running on.
        /// </summary>
        public string? RoleName { get; set; }
        /// <summary>
        /// Cloud role instance of what the application is running on.
        /// </summary>
        public string? RoleInstance { get; set; }
    }

    /// <summary>
    /// Information on the current operation in applciation.
    /// </summary>
    public class Operation
    {
        /// <summary>
        /// Application-defined unique Id for the top-most operation.
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// Id of the parent operation.
        /// </summary>
        public string? ParentId { get; set; }
        /// <summary>
        /// Application-defined source.
        /// </summary>
        public string? SyntheticSource { get; set; }
        /// <summary>
        /// Application-defined name of operation.
        /// </summary>
        public string? Name { get; set; }
    }

    /// <summary>
    /// Location information of who's accessing the applciation.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Ip address of where the application is being access from.
        /// </summary>
        public string? Ip { get; set; }
    }
}
