using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

using var httpClient = new HttpClient();
var isLoggedInResult = await httpClient.GetStringAsync("http://fritz.box/login_sid.lua");
var xml = XDocument.Parse(isLoggedInResult);
var challenge = xml.Descendants().First(d => d.Name == "Challenge")?.Value;
var hash = MD5.HashData(Encoding.Unicode.GetBytes($"{challenge}-{args[0]}"));
var loginRequest = "http://fritz.box/login_sid.lua?username=&response=" + challenge + "-" + Convert.ToHexString(hash).ToLower();
var loginResult = await httpClient.GetStringAsync(loginRequest);
xml = XDocument.Parse(loginResult);
var sid = xml.Descendants().First(d => d.Name == "SID").Value;
var formData = new FormUrlEncodedContent(new Dictionary<string, string>()
{
    { "sid",sid},
    { "device","16"},
    { "switch",args.Length==0?"0": args[1]},
    { "xhr","1"},
    { "useajax","1"},
});
var result = await httpClient.PostAsync("http://fritz.box/net/home_auto_overview.lua", formData);