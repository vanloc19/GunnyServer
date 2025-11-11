namespace SqlDataProvider.Data
{
    public class UserGmActivityCondition
    {
        public long ID { get; set; }
        public int UserID { get; set; }
        public string ActivityID { get; set; }
        public string GiftBagID { get; set; }
        public int StatusID { get; set; }
        public int StatusValue { get; set; }
    }
}
