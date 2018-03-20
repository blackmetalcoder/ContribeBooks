using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using ContribeBooks.models;
using System.Data;
using System.Configuration;
using System.Net.Mail;

namespace ContribeBooks
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IBookStoreService
    {
        List<Bocker> lstBook = new List<Bocker>();

        public string AddToChart(string sessionID, string Author, string Title, string Price)
        {
            string svar = "Added to chart";
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DbStore"].ToString()))
            {
                decimal pris = decimal.Parse(Price);
                                                         string sSQL = "INSERT INTO TempOrder (SessionID, Title, Author, Price) VALUES (@sessionID, @Author , @Title, @pris)";
                using (SqlCommand cmd = new SqlCommand(sSQL, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@sessionID", SqlDbType.NChar,100);
                    cmd.Parameters["@sessionID"].Value = sessionID;
                    cmd.Parameters.Add("@Author", SqlDbType.NChar, 100, Author);
                    cmd.Parameters["@Author"].Value = Author;
                    cmd.Parameters.Add("@Title", SqlDbType.NChar, 100, Title);
                    cmd.Parameters["@Title"].Value = Title;
                    cmd.Parameters.Add("@pris", SqlDbType.Float, 100);
                    cmd.Parameters["@pris"].Value = double.Parse(Price);
                    cmd.Connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                }
            }
            return svar;
        }

        public List<basket> getBasket(string sessionID)
        {
            List<basket> B = new List<basket>();
            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DbStore"].ToString()))
            {
                string sSQL = "SELECT * FROM TempOrder WHERE SessionID ='" + sessionID + "'";
                using (SqlCommand cmd = new SqlCommand(sSQL, con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        basket  b = new basket();
                        b.Author = reader["Author"].ToString().Trim();
                        b.Price = decimal.Parse(reader["Price"].ToString());
                        b.SessionID = reader["SessionID"].ToString().Trim();
                        b.Title = reader["Title"].ToString().Trim();
                        B.Add(b);
                    }
                    cmd.Connection.Close();
                }
            }
            return B;
        }

        public async Task<IEnumerable<Bocker>> GetBook(string search)
        {
            var url = "https://raw.githubusercontent.com/contribe/contribe/dev/arbetsprov-net/books.json";
            var books = BookSource._download_json<CListBooks>(url);
            var V = books.books;
            foreach (var item in V)
            {
                Bocker b = new Bocker
                {
                    Author = item.Author,
                    InStock = item.InStock,
                    Price = item.Price,
                    Title = item.Title
                };
                lstBook.Add(b);
            }
            //Nu söker vi
            var F = lstBook.FindAll(c => (c.Author == search) || (c.Title == search));
            return F;
        }

        public async Task<IEnumerable<Bocker>> GetBooks()
        {
            var utf8 = Encoding.UTF8;
            var url = "https://raw.githubusercontent.com/contribe/contribe/dev/arbetsprov-net/books.json";
            var books = BookSource._download_json<CListBooks>(url);
            var V = books.books;
            foreach (var item in V)
            {
                byte[] bytes = Encoding.Default.GetBytes(item.Title);
                string sTitle = Encoding.UTF8.GetString(bytes);
                Bocker b = new Bocker
                {
                    Author = item.Author,
                    InStock = item.InStock,
                    Price = item.Price,
                    Title = sTitle
                };
                lstBook.Add(b);
            }
            return lstBook;
        }

        public string OrderHead(string Name, string Adress, string Zip, string Email, string City, string sessionID)
        {
            string svar = string.Empty;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DbStore"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "AddOrderHead";
                    cmd.Connection = con;
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Name", Name));
                    cmd.Parameters.Add(new SqlParameter("@Adress", Adress));
                    cmd.Parameters.Add(new SqlParameter("@Mail", Email));
                    cmd.Parameters.Add(new SqlParameter("@Zip", Zip));
                    cmd.Parameters.Add(new SqlParameter("@City", City));
                    cmd.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    svar = cmd.Parameters["@id"].Value.ToString();
                    cmd.Parameters.Clear();
                    con.Close();
                }
            }
            int iID = int.Parse(svar);
            if (iID > 0)
            {
                using (SqlConnection con2 = new SqlConnection(ConfigurationManager.ConnectionStrings["DbStore"].ToString()))
                {
                    con2.Open();
                    string sSQL = "SELECT * FROM TempOrder WHERE SessionID = '" + sessionID + "'";
                    using (SqlCommand cmdBasket = new SqlCommand(sSQL, con2))
                    {
                        cmdBasket.CommandType = CommandType.Text;
                        SqlDataReader reader = cmdBasket.ExecuteReader();
                        while (reader.Read())
                        {
                            using (SqlCommand cmd = new SqlCommand())
                            {
                                cmd.CommandText = "addOrderRows";
                                cmd.Connection = con2;
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add(new SqlParameter("@OrderId", iID));
                                cmd.Parameters.Add(new SqlParameter("@Author", reader["Author"].ToString().Trim()));
                                cmd.Parameters.Add(new SqlParameter("@Title", reader["Title"].ToString().Trim()));
                                cmd.Parameters.Add(new SqlParameter("@Price", double.Parse(reader["Price"].ToString().Trim())));
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                        }
                    }
                    con2.Close();
                }
            }
            svar = sendOrderMail(Email, iID.ToString());
            return svar;
        }

        public string sendOrderMail(string Email, string orderID)
        {
            string sHTML = string.Empty;
            sHTML += "<h1>Order confirmation</h1><br/>";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DbStore"].ToString()))
            {
                string sOrderHead = "SELECT * FROM OrderHead WHERE id=" + orderID;
                using (SqlCommand cmdHead = new SqlCommand(sOrderHead, con))
                {
                    cmdHead.CommandType = CommandType.Text;
                    cmdHead.Connection.Open();
                    SqlDataReader readerHead = cmdHead.ExecuteReader();
                    while (readerHead.Read())
                    {
                        sHTML += "Order to: <br/>" + readerHead["Name"].ToString().Trim() + "<br/>";
                        sHTML += readerHead["Adress"].ToString().Trim() + " <br/> ";
                        sHTML += readerHead["Zip"].ToString().Trim() + ", " + readerHead["City"] + "<br/><br/>";
                    }
                }
            }
            string sOrderRows = "SELECT * FROM OrderRows WHERE OrderID=" + orderID;
            double total = 0;
            using (SqlConnection conRows = new SqlConnection(ConfigurationManager.ConnectionStrings["DbStore"].ToString()))
            {
                using (SqlCommand cmdRows = new SqlCommand(sOrderRows, conRows))
                {
                    cmdRows.CommandType = CommandType.Text;
                    cmdRows.Connection.Open();
                    SqlDataReader readerRows = cmdRows.ExecuteReader();
                    while(readerRows.Read())
                    {
                        total += double.Parse(readerRows["Price"].ToString());
                        double p = double.Parse(readerRows["Price"].ToString());
                        sHTML += readerRows["Author"].ToString().Trim() + ", " + readerRows["Title"].ToString().Trim() + ", " + p.ToString("#.##") + "<br/>";
                    }
                }
            }
            sHTML += "_________________________________________<br/>";
            sHTML += "<strong>Total: "  + total.ToString("#.##") + "<strong/>";
                string svar = string.Empty;
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.blacktv.se");

                mail.From = new MailAddress("black@blacktv.se");
                mail.To.Add(Email);
                mail.Subject = "Thanks for your order!";
                mail.IsBodyHtml = true;
                mail.Body = sHTML;
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("black@blacktv.se", "Olle8910");
                SmtpServer.Send(mail);
                svar = "mail Send";
            }
            catch (Exception ex)
            {
                svar = ex.Message;
            }
            return svar;
        }
    }
}
