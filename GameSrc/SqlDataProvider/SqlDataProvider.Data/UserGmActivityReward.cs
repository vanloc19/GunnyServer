namespace SqlDataProvider.Data
{
    public class UserGmActivityReward
    {
        public long ID { get; set; }
        public int UserID { get; set; }
        public string ActivityID { get; set; }
        public string GiftBagID { get; set; }
        public int Times { get; set; }
    }
}