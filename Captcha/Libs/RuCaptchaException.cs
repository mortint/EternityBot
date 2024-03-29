using System;

namespace Eternity.Captcha.capLib {
    public class RuCaptchaException : Exception {
        public RuCaptchaException() {
        }
        public RuCaptchaException(string message) : base(message) {
        }
        public RuCaptchaException(string message, Exception inner) : base(message, inner) {
        }
    }
}
