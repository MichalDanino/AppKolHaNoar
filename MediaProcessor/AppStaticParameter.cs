using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO;
using static DTO.Enums;

namespace MediaProcessor;
public class AppStaticParameter
{
    public static List<VideoDetails> videoDownLoad = new List<VideoDetails>();
    public static List<ChannelExtension> channels = new List<ChannelExtension>();
    public static eStatus globalStatus = eStatus.SUCCESS;
    public static List<string> forbiddenWords = new List<string>();
}
    

