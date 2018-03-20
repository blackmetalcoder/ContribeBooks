using ContribeBooks.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace ContribeBooks
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IBookStoreService
    {

        [OperationContract]
        Task<IEnumerable<Bocker>> GetBooks();

        [OperationContract]
        Task<IEnumerable<Bocker>> GetBook(string search);

        [OperationContract]
        string AddToChart(string sessionID, string Author, string Title, string Price);

        [OperationContract]
        List<basket> getBasket(string sessionID);

        [OperationContract]
        string OrderHead(string Name, string Adress, string Zip, string Email, string City, string sessionID);

        [OperationContract]
        string sendOrderMail(string Email, string orderID);

    }



}
