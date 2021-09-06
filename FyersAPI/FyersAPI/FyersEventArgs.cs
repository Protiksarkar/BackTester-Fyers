/*
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
    MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
    FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
    WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FyersAPI
{
    public class FyersEventArgs : EventArgs
    {
        public FyersEventArgs(MessageType messageType)
        {
            MessageType = messageType;
        }

        /// <summary>
        /// Gets the <see cref="MessageType"/>
        /// </summary>
        public MessageType MessageType { get; }
    }

    public class DataReceivedEventArgs : FyersEventArgs
    {
        public DataReceivedEventArgs(MessageType messageType, byte[] data) : base(messageType)
        {
            Data = data;
        }

        /// <summary>
        /// Gets the byte array
        /// </summary>
        public byte[] Data { get; }
    }

    public class MessageReceivedEventArgs : FyersEventArgs
    {
        public MessageReceivedEventArgs(MessageType messageType, string message) : base(messageType)
        {
            Message = message;
        }

        /// <summary>
        /// Gets the socket message
        /// </summary>
        public string Message { get; }
    }

    public class SocketErrorEventArgs : FyersEventArgs
    {
        public SocketErrorEventArgs(MessageType messageType, Exception exception) : base(messageType)
        {
            Exception = exception;
        }

        /// <summary>
        /// Gets the socket exception
        /// </summary>
        public Exception Exception { get; }
    }

}
