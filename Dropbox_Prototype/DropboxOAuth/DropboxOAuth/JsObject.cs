using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace DropBoxOAuth
{
    [Serializable]
    public class JsObject : Dictionary<string, object>
    {
        public JsObject()
        {

        }

        protected JsObject(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }


        /// <summary>
        /// Add the contents of the specified object under the specified optional name to the current JsObject, throwing an InvalidOperationException if duplicate entries exist.
        /// </summary>
        /// <param name="data">A JsObject to be aggregated with the JsObject.</param>
        public void AddObject(JsObject data)
        {

            foreach (KeyValuePair<string, object> entryPair in data)
            {
                if (this.ContainsKey(entryPair.Key))
                {
                    throw new InvalidOperationException("Duplicate entry found.  Key = " + entryPair.Key);
                }

                this[entryPair.Key] = entryPair.Value;
            }
        }
    }
}