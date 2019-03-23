using System.Text;
using pnyx.net.fluent;

namespace pnyx.net.util
{
    public class EncodingProxy : Encoding
    {
        private readonly Encoding encoding;
        private readonly Settings settings;

        public EncodingProxy(Encoding encoding, Settings settings) : base(encoding.CodePage, encoding.EncoderFallback, encoding.DecoderFallback)
        {
            this.encoding = encoding;
            this.settings = settings;
        }
        
        public override byte[] GetPreamble()
        {
            if (settings.outputByteOrderMarks)
                return encoding.GetPreamble();
            
            return new byte[0];
        }        

        public override int GetByteCount(char[] chars, int index, int count)
        {
            return encoding.GetByteCount(chars, index, count);
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            return encoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return encoding.GetCharCount(bytes, index, count);
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            return encoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        }

        public override int GetMaxByteCount(int charCount)
        {
            return encoding.GetMaxByteCount(charCount);
        }

        public override int GetMaxCharCount(int byteCount)
        {
            return encoding.GetMaxCharCount(byteCount);
        }

        public override string BodyName => encoding.BodyName;
        public override string EncodingName => encoding.EncodingName;
        public override string HeaderName => encoding.HeaderName;
        public override string WebName => encoding.WebName;
        public override int WindowsCodePage => encoding.WindowsCodePage;
        public override bool IsBrowserDisplay => encoding.IsBrowserDisplay;
        public override bool IsBrowserSave => encoding.IsBrowserSave;
        public override bool IsMailNewsDisplay => encoding.IsMailNewsDisplay;
        public override bool IsMailNewsSave => encoding.IsMailNewsSave;
        public override bool IsSingleByte => encoding.IsSingleByte;
        public override int CodePage => encoding.CodePage;
        public override bool IsAlwaysNormalized(NormalizationForm form) { return encoding.IsAlwaysNormalized(form); }
        public override Decoder GetDecoder() { return encoding.GetDecoder(); }
        public override Encoder GetEncoder() { return encoding.GetEncoder(); }
    }
}