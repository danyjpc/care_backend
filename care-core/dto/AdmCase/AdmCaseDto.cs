using System;
using care_core.dto.AdmOrganization;
using care_core.dto.AdmOrganizationMember;
using care_core.dto.AdmPerson;
using care_core.dto.AdmTypology;
using care_core.dto.AdmUser;
using care_core.model;
using care_core.util;

namespace care_core.dto.AdmCase
{
    public class AdmCaseDto
    {
        public AdmPersonDto victim { get; set; } = new AdmPersonDto();

        public long case_id { get; set; }

        public string aggressor_first_name { get; set; } = CareConstants.EMPTY_STRING;

        public string aggressor_last_name { get; set; } = CareConstants.EMPTY_STRING;

        public DateTime aggressor_birthday { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;

        public long aggressor_phone_number { get; set; } = CareConstants.ZERO_DEFAULT;

        public long aggressor_cui { get; set; }

        public AdmTypologyDto aggressor_state { get; set; } = new AdmTypologyDto() { };

        public string aggressor_address_line { get; set; } = CareConstants.EMPTY_STRING;

        public AdmTypologyDto aggressor_city { get; set; } = new AdmTypologyDto() { };

        public AdmTypologyDto aggressor_occupation { get; set; } = new AdmTypologyDto() { };

        public AdmTypologyDto aggressor_marital_status { get; set; } = new AdmTypologyDto() { };

        public string aggressor_work_place { get; set; } = CareConstants.EMPTY_STRING;

        public DateTime act_date { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;

        public string act_place { get; set; } = CareConstants.EMPTY_STRING;

        public AdmTypologyDto victim_relationship { get; set; } = new AdmTypologyDto() { };

        public string act_description { get; set; } = CareConstants.EMPTY_STRING;

        public AdmTypologyDto identified_violence_type { get; set; } = new AdmTypologyDto() { };

        public Boolean lives_with_aggressor { get; set; }

        public AdmTypologyDto accompaniment_type { get; set; } = new AdmTypologyDto() { };

        public AdmTypologyDto institution { get; set; } = new AdmTypologyDto() { };

        public AdmTypologyDto accompaniment_route { get; set; } = new AdmTypologyDto() { };

        public string annotation { get; set; } = CareConstants.EMPTY_STRING;

        public AdmOrganizationDto recording_organization { get; set; } = new AdmOrganizationDto() { };

        //TODO change to organizationContact
        public AdmOrganizationMemberDto recorded_by { get; set; }

        public AdmTypologyDto case_category { get; set; } = new AdmTypologyDto() { };

        public Boolean attached_victim_dpi { get; set; } = CareConstants.FALSE;

        public Boolean attached_victim_birth_certificate { get; set; } = CareConstants.FALSE;

        public Boolean attached_children_birth_certificate { get; set; } = CareConstants.FALSE;

        public Boolean attached_marriage_certificate { get; set; } = CareConstants.FALSE;

        public Boolean attached_aggressor_dpi { get; set; } = CareConstants.FALSE;

        public AdmTypologyDto status { get; set; } = new AdmTypologyDto() { };

        public AdmUserDto created_by { get; set; } = new AdmUserDto();

        public DateTime date_created { get; set; } = CareConstants.DATE_TIME_NO_TIMEZONE;

        public AdmCaseDto()
        {
        }

        public AdmCaseDto(long caseId, AdmPersonDto victim, string aggressorFirstName, string aggressorLastName,
            long aggressorPhoneNumber, long aggressorCui,
            string aggressorAddressLine, string aggressorWorkPlace, string actPlace, string actDescription,
            bool livesWithAggressor, string annotation,
            AdmOrganizationMemberDto recordedBy, bool attachedVictimDpi, bool attachedVictimBirthCertificate,
            bool attachedChildrenBirthCertificate, bool attachedMarriageCertificate,
            bool attachedAggressorDpi,
            AdmTypologyDto aggressorState,
            AdmTypologyDto aggressorCity,
            AdmTypologyDto aggressorOcupation,
            AdmTypologyDto aggressorMaritalStatus,
            AdmTypologyDto victimRelationship,
            AdmTypologyDto identifiedViolenceType,
            AdmTypologyDto accompanimentType,
            AdmTypologyDto institution,
            AdmTypologyDto accompanimentRoute,
            AdmTypologyDto caseCategory,
            AdmTypologyDto status,
            AdmOrganizationDto recordingOrganization,
            AdmUserDto createdBy,
            DateTime dateCreated,
            DateTime aggressorBirthday,
            DateTime actDate)
        {
            case_id = caseId;
            this.victim = victim;
            aggressor_first_name = aggressorFirstName;
            aggressor_last_name = aggressorLastName;
            aggressor_phone_number = aggressorPhoneNumber;
            aggressor_cui = aggressorCui;
            aggressor_address_line = aggressorAddressLine;
            aggressor_work_place = aggressorWorkPlace;
            act_place = actPlace;
            act_description = actDescription;
            lives_with_aggressor = livesWithAggressor;
            this.annotation = annotation;
            recorded_by = recordedBy;
            attached_victim_dpi = attachedVictimDpi;
            attached_victim_birth_certificate = attachedVictimBirthCertificate;
            attached_children_birth_certificate = attachedChildrenBirthCertificate;
            attached_marriage_certificate = attachedMarriageCertificate;
            attached_aggressor_dpi = attachedAggressorDpi;
            //case typologies
            aggressor_state = aggressorState;
            aggressor_city = aggressorCity;
            aggressor_occupation = aggressorOcupation;
            aggressor_marital_status = aggressorMaritalStatus;
            victim_relationship = victimRelationship;
            identified_violence_type = identifiedViolenceType;
            accompaniment_type = accompanimentType;
            this.institution = institution;
            accompaniment_route = accompanimentRoute;
            case_category = caseCategory;
            this.status = status;
            recording_organization = recordingOrganization;
            created_by = createdBy;
            date_created = dateCreated;
            aggressor_birthday = aggressorBirthday;
            act_date = actDate;
        }
        
        
    }
}