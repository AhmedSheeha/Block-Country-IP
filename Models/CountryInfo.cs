namespace Block_Country_IP.Models
{
    public class CountryInfo
    {
        public string ContryCode { get; set; }
        public string CountryName { get; set; }
        public string ISP {  get; set; }
        public DateTime? Date { get; set; }
        public bool Temp {  get; set; } = false;
        public DateTime? ReleaseTime { get; set; }
    }
}
