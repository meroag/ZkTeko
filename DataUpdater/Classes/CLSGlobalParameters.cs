using System.Collections.Generic;
using System.Data.SqlClient;

namespace DataUpdater.Classes
{
    /// <summary>
    /// -- ვერსია 0.6 -- 18 Oct 2015 19:00 ნიკა
    /// . Initialize-დან ამოვიღე GlobalParameters.Errors ის ინიციალიზაცია და გადავიტანე იქვე კლასში
    ///   GlobalParameters.InitializeErrorTexts()
    ///   Form1.cs-ის Load შია ჩასამატებელი ეს ფუქცია
    /// . დაემატა რამდენიმე StringParameters
    /// -- ვერსია 0.5 -- 7 Oct 2015 14:00 ნიკა
    /// </summary>

    public static class StringParameters
    {
        public static string CaptionAdd = "ჩამატება";
        public static string CaptionEdit = "რედაქტირება";
        public static string CaptionCopy = "კოპირება";
        public static string CaptionFind = "ძებნა";
        public static string CaptionError = "შეცდომა";

        public static string LookupEditNullText = "გთხოვთ არიჩიოთ სიიდან";
        public static string LookupEditNullTextReport = "გთხოვთ არიჩიოთ რეპორტის ფაილი";

        public static string GascorebaDgeError = "თქვენ არ შეგიძლიათ ამ თარიღით ინფორმაციის შეტანა!";

        public static string GascorebaSxvaError =
            "თქვენ არ შეგიძლიათ სხვა მომხმარებლის მიერ შეტანილი ინფორმაციის ცვლილება!";

        public static string ReportNoData = "რეპორტი არ შეიცავს მონაცემებს";
        public static string NoCompany = "კომპანიის რეკვიზიტები არაა განსაზღვრული";
        public static string NoParams = "ვერ ხერხდება პარამეტრების განსაზღვრა";
        public static string NoGroupBy = "გთხოვთ მონიშნოთ ველები დაჯგუფებისთვის";
        public static string GridColumnFreezeRight = "სვეტის გაყინვა მარჯვნივ";
        public static string GridColumnFreezeLeft = "სვეტის გაყინვა მარცხნივ";
        public static string GridColumnRemoveFreeze = "გაყინვის მოხსნა";
        public static string GridColumnRemoveFreezeAll = "გაყინვის მოხსნა ყველა სვეტზე";
        public static string GridColumnFilterMode = "ფილტრით რამდენიმეს ამორჩევის საშვალება";

        public static string RsConnectionEstablished = "კავშირი დამყარებულია";

        public static string RsConnectionFailedCheckUser =
            "ვერ მოხერხდა სისტემასთან კავშირი, შეამოწმეთ მომხმარებელი და პაროლი";

        public static string RsConnectionFailed = "ვერ მოხერხდა სისტემასთან კავშირი, შეამოწმეთ მომხმარებელი და პაროლი";

        public static string ChartOperationHorizontal = "ჰორიზონტალური(სტრიქონების მიხედვით)";
        public static string ChartOperationVertical = "ვერტიკალური(კოლონების მიხედვით)";

        public static string CaptionPeriodi = "პერიოდის არჩევა";
        public static string CaptionTxtData = "თარიღის არჩევა";
        public static string Today = "დღევანდელი";
        public static string Last3Days = "ბოლო 3 დღის";
        public static string LastWeek = "ბოლო კვირა";
        public static string LastMonth = "ბოლო თვე";
        public static string LastYear = "ბოლო წელი";
        public static string Everything = "სრული ინფორმაცია";

        public static string January = "იანვარი";
        public static string February = "თებერვალი";
        public static string March = "მარტი";
        public static string April = "აპრილი";
        public static string May = "მაისი";
        public static string June = "ივნისი";
        public static string July = "ივლისი";
        public static string August = "აგვისტო";
        public static string September = "სექტემბერი";
        public static string October = "ოქტომბერი";
        public static string November = "ნოემბერი";
        public static string December = "დეკემბერი";

        public static string Tanam = "თანამშრომელი";

        public static string StandartTime = "სტანდარტული დრო";
        public static string StandartRaod = "სტანდარტული რაოდენობა";

        public static string CaptionPanelLayoutOperations = "ძებნის ფილტრის სქემები";
        public static string CaptionGridLayoutOperations = "ცხრილის სქემები";
    }

    public class Language
    {
        public Language()
        {
            NameX = NameEng = NameGeo = "EmptyValue";
        }

        public Language(string nameGeo, string nameEng, string nameX)
        {
            NameGeo = nameGeo;
            NameEng = nameEng;
            NameX = nameX;
        }

        public string NameGeo { get; private set; }
        public string NameEng { get; private set; }
        public string NameX { get; private set; }
    }

    public static class GlobalParameters
    {
        //Translate
        public static Dictionary<string, Language> FieldContainerName =
            new Dictionary<string, Language>();

        public static Dictionary<string, Language> Languages =
            new Dictionary<string, Language>();

        public static Dictionary<string, string> Errors = new Dictionary<string, string>();

        public static string UserName;
        public static string DatabaseName;
        public static SqlConnection cn;
        public static string ConnStr;

        public static string FieldName;
        public static string TableName;

        public static int FindID;
        public static int IDForFind;

        public static bool FindFormsRestoreDgSettings = false;
        public static bool ViewFullLog;
        public static bool CreateLog;
        public static bool ClearDelReal;

        public static bool SetFont;
        public static string FontName;
        public static float FontSize;
        public static string ReportsFolder;
        public static string MonacemebiFolder;
        public static string MailFolder;
        public static string XMLFolder;
        public static string DateFormat;
        public static string ServerDateFormat = "MM/dd/yyyy";
        public static string ServerDateTimeFormat = "MM/dd/yyyy HH:mm:ss";
    }
}