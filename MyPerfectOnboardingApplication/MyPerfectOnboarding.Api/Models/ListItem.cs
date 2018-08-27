using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MyPerfectOnboarding.Api.Models
{
    [DataContract]
    public class ListItem
    {
        private Guid _id;
        private string _text;
        private bool _isActive;
        private DateTime _creationTime;
        private DateTime _lastUpdateTime;

        public ListItem(Guid id, string text, bool isActive, DateTime creationTime, DateTime lastUpdateTime)
        {
            this._id = id;
            this._text = text;
            this._isActive = isActive;
            this._creationTime = creationTime;
            this._lastUpdateTime = lastUpdateTime;
        }

        [DataMember]
        public Guid Id
        {
            get => _id;
            set => _id = value;
        }

        [DataMember]
        public string Text
        {
            get => _text;
            set => _text = value;
        }

        [DataMember]
        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }

        [DataMember]
        public DateTime CreationTime
        {
            get => _creationTime;
            set => _creationTime = value;
        }

        [DataMember]
        public DateTime LastUpdateTime
        {
            get => _lastUpdateTime;
            set => _lastUpdateTime = value;
        }
    }
}