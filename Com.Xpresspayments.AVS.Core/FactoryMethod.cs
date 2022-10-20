using Com.Xpresspayments.AVS.Services;
using Com.Xpresspayments.AVS.Services.Integrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Core
{
    public class FactoryImplemetation{
        private readonly FidelityBank _fidelityBank;
        private readonly GTBank _gtBank;
        private readonly Nibss _nibbs;
        private readonly UBA _uba;
        private readonly SterlingBank _sterlingBank;
        public FactoryImplemetation(FidelityBank fidelityBank , GTBank gtBank , Nibss nibbs , UBA uba, SterlingBank sterlingBank)
        {
            _uba = uba;
            _nibbs = nibbs;
            _gtBank = gtBank;
            _fidelityBank = fidelityBank;
            _sterlingBank = sterlingBank;
        }
        public IFactory FactoryConsumer(string providerName)
        {
            return providerName switch
            {
                "Fidelity" => _fidelityBank,
                "GTBank" => _gtBank,
                "Nibss" => _nibbs,
                "UBA" => _uba,
                "Sterling" => _sterlingBank,
                _ => throw new NotSupportedException($"AVS Doesn't have an implemetation for provider {providerName}"),
            };

            }
        }
    
    
}
