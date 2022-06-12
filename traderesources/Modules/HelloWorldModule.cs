using Yoda.Interfaces;
using Yoda.Interfaces.Menu;

namespace Yoda.Controllers {
    [DefaultProject("systemProject")]
    public class HelloWorldModule: YodaModule {
        public override string Text {
            get { return "Дефолтный конфигурационный модуль."; }
        }

        public override MenuLink[] Menu()
        {
            return new MenuLink[] {
            };
        }
    }
}