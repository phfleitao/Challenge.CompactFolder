using CompactFolder.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CompactFolder.Domain.ValueObjects
{
    public class TEmail : ValueObject
    {
        private static readonly Regex EmailRegex = new Regex(
        @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
        RegexOptions.IgnoreCase);

        public string Address { get; }

        private TEmail(string address)
        {
            Validate(address);
            
            Address = address;
        }
        public static TEmail Create(string address)
        {
            return new TEmail(address);
        }
        public static implicit operator TEmail(string address)
        {
            return TEmail.Create(address);
        }

        private void Validate(string address)
        {
            if (string.IsNullOrWhiteSpace(address) ||
                !EmailRegex.IsMatch(address))
                throw new ArgumentException("Invalid email address:", nameof(address));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Address.ToLowerInvariant();
        }
        public override string ToString()
        {
            return this.Address;
        }
    }
}
