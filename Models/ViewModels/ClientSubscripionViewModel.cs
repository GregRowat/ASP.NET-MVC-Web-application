namespace Lab4.Models.ViewModels
{
    public class ClientSubscriptionsViewModel
    {
        public Client Client { get; set; }
        public IEnumerable<NewsBoard> NewsBoards { get; set; }
    }

}
