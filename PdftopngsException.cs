using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RunPdftopngs
{
    [Serializable]
    public class PdftopngsException : Exception, ISerializable
    {
        public PdftopngsException(int errorCode, string errorMessage)
            : base($"pdftopngs failed in {errorCode}: {errorMessage}")
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public int ErrorCode { get; }
        public string ErrorMessage { get; }

        protected PdftopngsException(
            System.Runtime.Serialization.SerializationInfo serializationInfo,
            System.Runtime.Serialization.StreamingContext streamingContext
        )
            : base(serializationInfo, streamingContext)
        {
            ErrorCode = serializationInfo.GetInt32("ErrorCode");
            ErrorMessage = serializationInfo.GetString("ErrorMessage");
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ErrorCode", ErrorCode);
            info.AddValue("ErrorMessage", ErrorMessage);
        }
    }
}
