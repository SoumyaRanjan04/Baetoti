using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
	[Table("Country", Schema = "baetoti")]
	public partial class Country : BaseEntity
	{
		public string CountryName { get; set; }

		public string CountryCode { get; set; }

		public string PhoneCode { get; set; }

		public int CountryStatus { get; set; }

        public DateTime RecordDateTime { get; set; }

    }
}
