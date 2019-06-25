using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ServiceOfOrder
{
    [Serializable]
    [XmlRoot("Order")]
    class Order
    {
        string clientId;
        long requestId;
        string name;
        int quantity;
        double price;
        bool correct;

     //   [XmlElement("cleintId")]
        public string ClientId { get { return clientId; } }
        [XmlElement("requestId")]
        public long RequestId { get { return requestId; } }
        [XmlElement("name")]
        public string Name { get { return name; } }
        [XmlElement("quantity")]
        public int Quantity { get { return quantity; } }
        [XmlElement("price")]
        public double Price { get { return price; } }
        [XmlElement("ifcorrect")]
        public bool IfCorrect { get { return correct; } }

        public Order(string clientId, long requestId, string name, int quantity, double price)
        {
            correct = true;

            if ((clientId.Length <= 6) && (!clientId.Contains(" ")))
                this.clientId = clientId;
            else
                { correct = false;
                clientId = " ";
                } 

            this.requestId= requestId;

            if (name.Length <= 255 )
               this.name= name;
            else
            {
                correct = false;
                name = " ";
            }

            this.quantity= quantity;
            this.price=price;
        }

        public T Load<T>(string FileSpec)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));

            using (FileStream aFile = new FileStream(FileSpec, FileMode.Open))
            {
                byte[] buffer = new byte[aFile.Length];
                aFile.Read(buffer, 0, (int)aFile.Length);

                using (MemoryStream stream = new MemoryStream(buffer))
                {
                    return (T)formatter.Deserialize(stream);
                }
            }



        }

         
    }
}
