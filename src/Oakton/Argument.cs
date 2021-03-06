using System;
using System.Collections.Generic;
using System.Reflection;
using Baseline;
using Baseline.Conversion;
using Oakton.Parsing;
using Oakton.Reporting;

namespace Oakton
{
    public class Argument : TokenHandlerBase
    {
        private readonly MemberInfo _member;
        private bool _isLatched;
        protected Func<string, object> _converter;

        public Argument(MemberInfo member, Conversions conversions) : base(member)
        {
            _member = member;
            _converter = conversions.FindConverter(member.GetMemberType());
        }

        public ArgumentReport ToReport()
        {
            return new ArgumentReport
            {
                Description = Description,
                Name = _member.Name.ToLower()
            };
        }

        public override bool Handle(object input, Queue<string> tokens)
        {
            if (_isLatched) return false;

            if (tokens.NextIsFlag()) return false;

            var value = _converter(tokens.Dequeue());
            setValue(input, value);

            _isLatched = true;

            return true;
        }

        public override string ToUsageDescription()
        {
            var memberType = _member.GetMemberType();
            if (memberType.GetTypeInfo().IsEnum)
            {
                return Enum.GetNames(memberType).Join("|");
            }

            return $"<{_member.Name.ToLower()}>";
        }
    }
}