using System;
using AutoMapper;
using Consent_Aries_VC.Data.DTO.Request;
using Consent_Aries_VC.Data.DTO.Response;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Aries.Storage;

namespace Consent_Aries_VC.Infrastructure.Configs {
    public class MappingProfileConfiguration : Profile {
        public MappingProfileConfiguration() {
            CreateMap<CreateAgentRequest, AgentOptions>()
                .ForMember(
                    dest => dest.WalletConfiguration,
                    map => map.MapFrom(src => new WalletConfiguration {
                        Id = src.WalletConfigurationId
                    })
                )
                .ForMember(
                    dest => dest.WalletCredentials,
                    map => map.MapFrom(src => new WalletCredentials {
                        Key = src.WalletKey
                    })
                );

            CreateMap<ProofRecord, ProofRecordDTO>()
                .ForMember(
                    dest => dest.State,
                    map => map.MapFrom(src => Enum.GetName(typeof(ProofState), src.State))
                );
        }
    }
}