using System;
using System.Collections.Generic;
using System.Text;

namespace Launcher
{
    public partial class GameConfig
    {
#if FRXX_KU25
        public const string LOGIN_URL = "http://www.ku25.com/client/frxx";
#elif FRXX_37TANG
        public const string LOGIN_URL = "http://extra.37tang.com/url/weiduan.php?gn=frxx";
#elif FRXX_43U
        public const string LOGIN_URL = "http://frxx.43u.com/client";
#elif FRXX_1912YX
        public const string LOGIN_URL = "http://frxx.1912yx.com/client";
#elif FRXX_ZIXIA
        public const string LOGIN_URL = "https://apps.zixia.com/frxx/index.php";
#elif FRXX_52GG
        public const string LOGIN_URL = "http://frxx.52gg.com/client";
#elif FRXX_1771WAN
        public const string LOGIN_URL = "http://frxx.1771wan.com/client";
#elif FRXX_YXA9
        public const string LOGIN_URL = "http://www.yxa9.com/client/frxx/index.php";
#elif FRXX_XINGDIE
        public const string LOGIN_URL = "https://www.ufojoy.com/pc/index.phtml?game=frxx";
#elif FRXX_YILING
        public const string LOGIN_URL = "http://frxx.10hud.com/logger";
#elif FRXX_45YX
        public const string LOGIN_URL = "https://www.45yx.com/client/login/1254?w=590&h=480&t=2";
#elif FRXX_45YX_160
        public const string LOGIN_URL = "http://play.160sh.com/wd/frxx/index.htm";
#elif FRXX_DY1
        public const string LOGIN_URL = "http://play.no1yx.com/wd/wd-frxx/index.htm";
#elif FRXX_DY2
        public const string LOGIN_URL = "http://www.30756.cn//client/common/index.php?gid=511";
#elif FRXX_8090
        public const string LOGIN_URL = "http://dlqxz.8090.com/frxx/login/index.php";
#elif FRXX_335WAN
        public const string LOGIN_URL = "https://web.28zhe.com/index/microgame/index?id=49";
#elif FRXX_JIUHOU
        public const string LOGIN_URL = "http://api.9hou.com/api/client/?gid=1119";
#elif FRXX_SHUNGWANG
        public const string LOGIN_URL = "https://gamesite.swjoy.com/embed/5852";
#elif FRXX_4YX
        public const string LOGIN_URL = "http://www.youxilifang.com/min/frxx";
#elif FRXX_BBJ
        public const string LOGIN_URL = "http://frxx.g1.10hud.com/logger";//和yiling一样的参数
#elif FRXX_FLASH
        public const string LOGIN_URL = "";
#elif FRXX_AQY
        public const string LOGIN_URL = "";
#else
        public const string LOGIN_URL = "https://gamesite.swjoy.com/embed/5852";

#endif
        public static string CHANNEL_LOGIN_URL
        {
            get
            {
                return LOGIN_URL;
            }
        }
        public static bool Is4YX()
        {
#if FRXX_4YX
            return true;
#endif
            return false;
        }
        /// <summary>
        /// 顺网
        /// </summary>
        /// <returns></returns>
        public static bool IsSW()
        {
#if FRXX_SHUNGWANG
            return true;
#endif
            return false;
        }
    }
}