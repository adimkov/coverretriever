using System;
using System.Windows;

using CoverRetriever.Model;

namespace CoverRetriever.SampleData
{
	public class RemoteCoverData : RemoteCover
	{
		public RemoteCoverData()
			: base(
			"123456789dsd",
			new Uri("http://t3.gstatic.com/images?q=tbn:ANd9GcTMMPCYjolqkR1vYJ_XRrKP-t9pu5YiSbC5iqMtO0sLL--t_3X4pw"),
			new Size(640, 480),
			new Uri("http://www.google.com.ua/imgres?imgurl=http://apod.nasa.gov/apod/image/0411/dr6_spitzer.jpg&imgrefurl=http://apod.nasa.gov/apod/ap041101.html&usg=__-elY6mHSPtbEvZC5sevO8uNK-qs=&h=480&w=640&sz=99&hl=ru&start=0&sig2=9r6aOlbGa3v3R3lt9I3wrA&zoom=1&tbnid=F9-nqniYLj_n0M:&tbnh=141&tbnw=184&ei=_U9MTdb9OsnqOa_1megP&prev=/images%3Fq%3D6%26hl%3Dru%26client%3Dfirefox-a%26hs%3Dram%26sa%3DX%26rls%3Dorg.mozilla:en-US:official%26biw%3D1274%26bih%3D879%26tbs%3Disch:1%26prmd%3Divns&itbs=1&iact=hc&vpx=440&vpy=541&dur=1101&hovh=194&hovw=259&tx=82&ty=218&oei=_U9MTdb9OsnqOa_1megP&esq=1&page=1&ndsp=35&ved=1t:429,r:23,s:0"), 
			new Size(140,140))
		{
			
		}
	}
}