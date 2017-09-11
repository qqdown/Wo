namespace Wo.KancolleCore
{
    class KancolleUtils
    {
        public const int Version = 1;
        public const string UpdateTime = "20161212";
        public const string KanColleUrl = @"http://www.dmm.com/netgame/social/-/gadgets/=/app_id=854854/";
        public const string KanColleFrameSrcPrefix = @"http://osapi.dmm.com/gadgets/ifr?synd=dmm&container=dmm";
        public const string KanColleAPIUrl = @"http://203.104.248.135/kcsapi/";
        public const string KanColleAPIKeyword = @"/kcsapi/";
        public const string KanColleSwfUrl = @"http://203.104.248.135/kcs/";

        /// <summary>
        /// 头衔
        /// </summary>
        public static string[] RankText { get; } = { "", "元帥", "大将", "中将", "少将", "大佐", "中佐", "新米中佐", "少佐", "中堅少佐", "新米少佐" };

        /// <summary>
        /// 获得URL当中的API信息
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetAPI(string url)
        {
            int index = url.IndexOf(KanColleAPIKeyword);
            if (index > 0)
            {
                return url.Substring(index + KanColleAPIKeyword.Length);
            }
            return "";
        }
    }
}
