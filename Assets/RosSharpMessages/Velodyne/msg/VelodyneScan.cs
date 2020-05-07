/* 
 * This message is auto generated by ROS#. Please DO NOT modify.
 * Note:
 * - Comments from the original code will be written in their own line 
 * - Variable sized arrays will be initialized to array of size 0 
 * Please report any issues at 
 * <https://github.com/siemens/ros-sharp> 
 */

using Newtonsoft.Json;

using RosSharp.RosBridgeClient.MessageTypes.Std;

namespace RosSharp.RosBridgeClient.MessageTypes.Velodyne
{
    public class VelodyneScan : Message
    {
        [JsonIgnore]
        public const string RosMessageName = "velodyne_msgs/VelodyneScan";

        //  Velodyne LIDAR scan packets.
        public Header header;
        //  standard ROS message header
        public VelodynePacket[] packets;
        //  vector of raw packets

        public VelodyneScan()
        {
            this.header = new Header();
            this.packets = new VelodynePacket[0];
        }

        public VelodyneScan(Header header, VelodynePacket[] packets)
        {
            this.header = header;
            this.packets = packets;
        }
    }
}
