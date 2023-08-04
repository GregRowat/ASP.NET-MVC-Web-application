namespace Lab4.Models
{
    public class Subscription
    {

        public int ClientId { get; set; }
        public String NewsBoardId { get; set; }
        public NewsBoard NewsBoard { get; set; }
        public Client Client { get; set; }

    }
}
