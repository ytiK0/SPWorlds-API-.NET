using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using forms;

namespace SP_api
{
    public class SPapi
    {
        private string baseURL = ("https://spworlds.ru/api/public/");
        private string key;
        public HttpClient client;

        /// <summary>
        /// Создать экземпляр класса SPapi
        /// </summary>
        /// <param name="id">id вашей карты</param>
        /// <param name="token">Токен вашей карты</param>
        public SPapi(string id, string token)
        {
            key = Base64Encode(id + ":" + token);
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
        }
        /// <summary>
        /// Получение ссылки на оплату
        /// </summary>
        /// <param name="amount">Стоимость покупки</param>
        /// <param name="redirectUrl">URL на которуюю попадёт пользователь после оплаты(по умолчанию spworlds.ru)</param>
        /// <param name="webhookUrl">URL куда сервер направит запрос, чтобы оповестить ваш сервер об успешной оплате</param>
        /// <param name="data">Строка до 100 символов, сюда можно поместить любые полезные данных</param>
        /// <returns>Ссылка на оплату</returns>
        public string GetPaymentLink(int amount, string webhookUrl, string data, string redirectUrl = "https://spworlds.ru")
        {
            if (data.Length > 100) return "Cтрока data содержит больше 100 символов";
            PaymentForm payForm = new PaymentForm(amount, redirectUrl, webhookUrl, data);
            var paymentRequest = new StringContent(JsonConvert.SerializeObject(payForm), Encoding.UTF8, "application/json");
            var jsonAnswer = client.PostAsync("https://spworlds.ru/api/public/payment", paymentRequest).Result.Content.ReadAsStringAsync().Result;
            var url = JsonConvert.DeserializeObject<PaymentUrlAnswer>(jsonAnswer).url;
            return url == null ? "Ошибка! Возможно ошибка в токене или id карты." : url;
        }
        /// <summary>
        /// Получение баланса карты
        /// </summary>
        /// <returns>Числовое значение баланса карты</returns>
        public int GetBalance()
        {
            int balance = -1;
            var jsonAnswer = client.GetAsync(baseURL + "card").Result.Content.ReadAsStringAsync().Result;
            balance = JsonConvert.DeserializeObject<BalanceAnswer>(jsonAnswer).balance;
            return balance;
        }
        /// <summary>
        /// Получение имени пользователя игрока
        /// </summary>
        /// <param name="discordId">id игрока Discord</param>
        /// <returns>Имя пользователя игрока</returns>
        public string GetUsername(string discordId)
        {
            var jsonAnswer = client.GetAsync(baseURL + "users/" + discordId).Result;
            var username = JsonConvert.DeserializeObject<UsernmeAnswer>(jsonAnswer.Content.ReadAsStringAsync().Result).username;
            return username == null ? "Пользователь не найден." : username;
        }
        /// <summary>
        /// Совершить перевод
        /// </summary>
        /// <param name="receiver">Номер карты получателя</param>
        /// <param name="amount">Количество АР</param>
        /// <param name="comment">Любой коментарий к переводу</param>
        /// <returns>Статус перевода</returns>
        public string Pay(string receiver, int amount, string comment)
        {
            if (amount <= 0) return "Количество переводимых АРов не может быть равно нулю либо меньше нуля.";
            HttpContent paymentRequest = new StringContent(JsonConvert.SerializeObject(new PayForm(receiver, amount, comment)), Encoding.UTF8, "application/json");
            var status = client.PostAsync(baseURL + "transactions", paymentRequest).Result.StatusCode;
            return status != System.Net.HttpStatusCode.OK ? "Ошибка! Возможно такого номера карты нет." : "Успешно!";
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}