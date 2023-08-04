namespace Lab4.Models.ViewModels
{
    // view model object to store and access list structures of built instances in the controllers
    public class NewsBoardViewModel
    {
        public IEnumerable<Client> Clients { get; set; }
        public IEnumerable<NewsBoard> NewsBoards { get; set;}
        public IEnumerable<Subscription> Subscriptions { get; set; }    
    }
}
