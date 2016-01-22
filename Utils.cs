public static class Utils
    {
        public static int queryStringID = 0;

        /// <summary>
        /// StoredProcedure dan gelen sonuçları pipeline ayıracı ile split edip string array olarak dönüş yapar.
        /// </summary>
        /// <param name="result">StoredProcedure dan dönen değer</param>
        /// <param name="splitKarakter"></param>
        /// <returns>string array</returns>
        public static string[] SplitStrings(string result, char splitKarakter)
        {
            string[] resultdizi = result.Split(splitKarakter);
            return resultdizi;
        }

        /// <summary>
        /// MemberShip kütüphanesi kullanılarak içerisinde en az bir tane alfanumeric karakter bulunan ve 8 karakterden oluşan rastgele parola oluşturur.
        /// </summary>
        /// <returns>string Parola</returns>
        public static string ParolaOlustur()
        {
            //return System.Web.Security.Membership.GeneratePassword(8, 1);
            string karakterler = "abcdefghijkmnoprstuvyz123456789";
            char[] chars = new char[7];
            Random rd = new Random();

            for (int i = 0; i < 7; i++)
            {
                chars[i] = karakterler[rd.Next(0, karakterler.Length)];
            }

            return new string(chars);
        }

        /// <summary>
        /// Kaydı yapılmak istenen kişinin yaşını bulur.
        /// </summary>
        /// <param name="dogumTarihi"></param>
        /// <returns></returns>
        public static int YasHesapla(DateTime dogumTarihi)
        {
            DateTime now = DateTime.Now;
            int age = now.Year - dogumTarihi.Year;
            if (now.Month < dogumTarihi.Month || (now.Month == dogumTarihi.Month && now.Day < dogumTarihi.Day)) age--;
            return age;
        }

        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "FAPV4AYBHI58687";

            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);

            using (Aes encryptor = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }

                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }

            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            try
            {
                string EncryptionKey = "FAPV4AYBHI58687";
                cipherText = cipherText.Replace(" ", "+");
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (Aes encryptor = Aes.Create())
                {
                    var pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);

                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return cipherText;
        }

        public static bool TcValidate(string tcKimlikNo)
        {
            bool returnvalue = false;
            if (tcKimlikNo.Length == 11)
            {
                Int64 ATCNO, BTCNO, TcNo;
                long C1, C2, C3, C4, C5, C6, C7, C8, C9, Q1, Q2;

                TcNo = Int64.Parse(tcKimlikNo);

                ATCNO = TcNo / 100;
                BTCNO = TcNo / 100;

                C1 = ATCNO % 10;
                ATCNO = ATCNO / 10;
                C2 = ATCNO % 10;
                ATCNO = ATCNO / 10;
                C3 = ATCNO % 10;
                ATCNO = ATCNO / 10;
                C4 = ATCNO % 10;
                ATCNO = ATCNO / 10;
                C5 = ATCNO % 10;
                ATCNO = ATCNO / 10;
                C6 = ATCNO % 10;
                ATCNO = ATCNO / 10;
                C7 = ATCNO % 10;
                ATCNO = ATCNO / 10;
                C8 = ATCNO % 10;
                ATCNO = ATCNO / 10;
                C9 = ATCNO % 10;
                ATCNO = ATCNO / 10;
                Q1 = ((10 - ((((C1 + C3 + C5 + C7 + C9) * 3) + (C2 + C4 + C6 + C8)) % 10)) % 10);
                Q2 = ((10 - (((((C2 + C4 + C6 + C8) + Q1) * 3) + (C1 + C3 + C5 + C7 + C9)) % 10)) % 10);

                returnvalue = ((BTCNO * 100) + (Q1 * 10) + Q2 == TcNo);
            }
            return returnvalue;
        }

        public static object Tipe<T>(this object obje, T tip)
        {
            var gecici = Activator.CreateInstance(Type.GetType(tip.ToString()));
            foreach (PropertyInfo pi in obje.GetType().GetProperties())
            {
                try
                {
                    gecici.GetType().GetProperty(pi.Name).SetValue(gecici, pi.GetValue(obje, null), null);
                }
                catch { }
            }
            return gecici;
        }

        public static object AnonimDenTypeACevir<T>(this List<T> liste, Type t)
        {
            var jenerikTip = typeof(List<>).MakeGenericType(t);
            var tipitip = Activator.CreateInstance(jenerikTip);
            MethodInfo metotEkle = tipitip.GetType().GetMethod("Add");
            foreach (T parca in liste) metotEkle.Invoke(tipitip, new object[] { parca.Tipe(t) });
            return tipitip;
        }

        /// <summary>
        /// QueryString den gelen değeri kontrol eder eğer INT değerden farklı bir değer gelirse
        /// sayfanın yönlendirilmesi için FALSE döner.
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns>TRUE geçerli querystring / FALSE geçersiz querystring</returns>
        public static bool QueryStringKontrol(string queryString)
        {
            bool retval = true;
            bool isNumeric = false;

            int n;
            if (queryString != null)
                isNumeric = !int.TryParse(queryString, out n);

            queryString = isNumeric ? Base64SifreCoz(queryString) : null;
            if (!string.IsNullOrEmpty(queryString))
            {
                if (!Int32.TryParse(queryString, out queryStringID))
                    retval = false;
            }
            return retval;
        }

        public static string Base64Sifrele(string metin)
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(metin))
            {
                byte[] data = UnicodeEncoding.UTF8.GetBytes(metin);
                Base64Encoder myEncoder = new Base64Encoder(data);

                sb.Append(myEncoder.GetEncoded());
            }
            return sb.ToString();
        }

        public static string Base64SifreCoz(string metin)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(metin))
            {
                char[] data = metin.ToCharArray();
                Base64Decoder myDecoder = new Base64Decoder(data);


                byte[] temp = myDecoder.GetDecoded();
                sb.Append(UTF8Encoding.UTF8.GetChars(temp));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Girilen mail adresini kontrol eder
        /// </summary>
        /// <param name="email"></param>
        /// <returns>bool</returns>
        public static bool IsEmail(string email)
        {
            string MatchEmailPattern =
                @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

            if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
            else return false;
        }

        /// <summary>
        /// MaskedEditExtender ile formatlı telefon numarasını temizler
        /// (545)-281-07-27 şeklinde ki telefonu 5452810727 şeklinde temizler
        /// </summary>
        /// <param name="tel">Telefon No</param>
        /// <returns></returns>
        public static string TelDuzenle(string tel)
        {
            tel = tel.Replace("(", "");
            tel = tel.Replace(")", "");
            tel = tel.Replace("-", "");
            tel = tel.Replace("_", "");
            tel = tel.Replace(" ", "");
            return tel;
        }

        /// <summary>
        /// HTML içeriklerinde html taglarını temizler
        /// </summary>
        /// <param name="html">HTML İçerik</param>
        /// <param name="acceptableTags">"script|link|title|p|b"</param>
        /// <returns></returns>
        public static string ClearHtmlTag(string html, string acceptableTags = "")
        {
            string stringPattern;

            if (acceptableTags.Length > 0)
                stringPattern = @"</?(?(?=" + acceptableTags + @")notag|[a-zA-Z0-9]+)(?:\s[a-zA-Z0-9\-]+=?(?:(["",']?).*?\1?)?)*\s*/?>";
            else
                stringPattern = @"<.*?>";

            return Regex.Replace(html, stringPattern, String.Empty, RegexOptions.Multiline);
        }
    }
