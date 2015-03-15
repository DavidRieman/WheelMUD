
namespace WheelMUD.Server
{
    using WheelMUD.Core;

    /// <summary>
    /// The 'client conected' event handler delegate.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The connection arguments of the event.</param>
    public delegate void ClientConnectedEventHandler(object sender, ConnectionArgs args);

    /// <summary>
    /// The 'client disconnected' event handler delegate.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The connection arguments of the event.</param>
    public delegate void ClientDisconnectedEventHandler(object sender, ConnectionArgs args);

    /// <summary>
    /// The 'input received' event handler delegate.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="input">The input that was received.</param>
    public delegate void InputReceivedEventHandler(object sender, string input);

    /// <summary>
    /// The 'data received' event handler delegage.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The connection arguments of the event.</param>
    public delegate void DataReceivedEventHandler(object sender, ConnectionArgs args);

    /// <summary>
    /// The 'data sent' event handler delegate.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="args">The connection arguments of the event.</param>
    public delegate void DataSentEventHandler(object sender, ConnectionArgs args);
}
