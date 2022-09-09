//В этом пространстве имён собранны классы для стерилизации и дестерилизации JSON файлов использующий класс SPapi
namespace forms
{
    //форма для стерелизации JSON для запроса сслки для оплаты
    internal class PaymentForm
    {
        public int amount { get; private set; }
        public string redirectUrl { get; private set; }
        public string webhookUrl { get; private set; }
        public string data { get; private set; }

        public PaymentForm(int amount, string redirectUrl, string webhookUrl, string data)
        {
            this.amount=amount;
            this.redirectUrl=redirectUrl;
            this.webhookUrl=webhookUrl;
            this.data=data;
        }
    }
    //форма для стерелизации JSON для совершения переводов
    internal class PayForm
    {
        public string receiver { get; set; }
        public int amount { get; set; }
        public string comment { get; set; }

        public PayForm(string receiver, int amount, string data)
        {
            this.receiver=receiver;
            this.amount=amount;
            this.comment=data;
        }
    }

    //формы для дестерелизации ответов сервера
    internal class PaymentUrlAnswer
    {
        public string url { get; set; }
        public PaymentUrlAnswer(string url) { this.url=url; }
    }
    internal class BalanceAnswer
    {
        public int balance { get; set; }
        public BalanceAnswer(int balance) { this.balance=balance; }
    }
    internal class UsernmeAnswer
    {
        public string username { get; set; }
        public UsernmeAnswer(string username) { this.username=username; }
    }
}