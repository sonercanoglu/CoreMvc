using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreMvc.Services
{
    public class Utils
    {
        public static string GetMessage(MessageType messageType, string entity)
        {
            string message = "";

            switch (messageType)
            {
                case MessageType.Add:       return entity + "'s Informations Created Succesfully";
                case MessageType.Update:    return entity + "'s Informations Updated Succesfully";
                case MessageType.Delete:    return entity + "'s Informations Deleted Succesfully";
                default:
                    break;
            }

            return message;
        }
    }

    public enum MessageType
    {
        Add = 1,
        Update = 2,
        Delete = 3,
    }
}
