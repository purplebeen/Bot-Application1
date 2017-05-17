using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using SchoolMeal;
using System.Xml;
using System.Collections;

namespace Bot_Application1.Dialogs
{

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        string id = "";
        public Task StartAsync(IDialogContext context)
        {

            context.Wait(showMenu);

            //context.Wait(MessageReceivedAsync);



            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            await context.PostAsync($"당신은 {activity.Text}라고 말했습니다. 총 {length} 글자 입니다");//output

            context.Wait(MessageReceivedAsync);
        }


        private async Task inputId(IDialogContext context, IAwaitable<object> result)
        {

            var activity = await result as Activity;
            //output
            // calculate something for us to return
            id = activity.Text;

            // return our reply to the user
            await context.PostAsync($"안녕하십니까?{id}님 어서오십시오.");//output
            await context.PostAsync($"실행하실 동작을 선택해 주세요.");
            await context.PostAsync("1.오늘의급식 \n\n 2.오늘의날씨 \n\n 3.나는누구야? \n\n 4.나가기\n\n");
            context.Wait(printMenu);
        }



        private async Task showMenu(IDialogContext context, IAwaitable<object> result)
        {
            //output
            var activity = await result as Activity;

            // calculate something for us to return
            if (activity.Text.Equals("챗봇아"))
            {
                await context.PostAsync($"안녕하십니까 챗봇 MM입니다");
                await context.PostAsync($"당신의 아이디는 무엇입니까?");
                context.Wait(inputId);
            }
            else
            {
                await context.PostAsync($"응답하지 않습니다. 챗봇아 라고 불러주세요!");
                context.Wait(showMenu);
            }
            // return our reply to the user
            //await context.PostAsync($"안녕하십니까?{id}님 어서오십시오.");//output

            // context.Wait(MessageReceivedAsync);
        }
        private async Task printMenu(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Text.Equals("1"))
            {
                int dateCount = 1;
                await context.PostAsync("오늘의 급식은.. \n");
                Meal meal = new Meal(Regions.Seoul, SchoolType.High, "B100000662");
                string date = DateTime.Now.ToString("dd");
                var menu = meal.GetMealMenu();

                foreach (var item in menu)
                {
                    if (dateCount == int.Parse(date)) { await context.PostAsync(item.ToString()); break; }
                    else { dateCount++; }
                    //noe노예
                    //
                }
                await context.PostAsync("");
                context.Wait(print);
                
            }


            else if(activity.Text.Equals("2"))
            {
                await context.PostAsync("마포구 아현동의 오늘 날씨입니다.");

                //날씨 파싱
                string url = "http://www.kma.go.kr/wid/queryDFSRSS.jsp?zone=1144055500";
                XmlDocument document = new XmlDocument();
                document.Load(url);
                XmlElement root = document.DocumentElement;
                XmlNodeList days = root.SelectNodes("//data/day");
                XmlNodeList hours = root.SelectNodes("//data/hour");
                XmlNodeList temporatures = root.SelectNodes("//data/temp");
                XmlNodeList weathers = root.SelectNodes("//data/wfKor");
                XmlNodeList rains = root.SelectNodes("//data/reh");
                string temp = "";


                for (int i = 0; i < days.Count; i++)
                {
                    if (days.Item(i).InnerText.Equals("0"))
                    {
                        temp += hours.Item(i).InnerText + "시 " + temporatures.Item(i).InnerText + "℃ " + weathers.Item(i).InnerText + " 강수확률 : " + rains.Item(i).InnerText + "\n\n";
                    }
                }
                await context.PostAsync(temp);
                context.Wait(print);
            }


            else if(activity.Text.Equals("3"))
            {
                await context.PostAsync($"{id} 님 입니다.");
                context.Wait(print);
            }


            else if (activity.Text.Equals("4"))
            {
                context.Wait(showMenu);
            }
        }

        private async Task print(IDialogContext context, IAwaitable<object> result) {
            await context.PostAsync($"{id}님");//output
            await context.PostAsync($"실행하실 동작을 선택해 주세요.");
            await context.PostAsync("1.오늘의급식 \n\n 2.오늘의날씨 \n\n 3.나는누구야? \n\n 4.나가기\n\n");
            context.Wait(printMenu);

        }

    }
}