using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff.Framework
{
    public interface IRiffConfigurableSettings
    {
        IDictionary<String, String> SupportedApplications();
    }
}
