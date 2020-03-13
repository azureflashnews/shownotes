# Azure Flash News Episode #88 - 03/13/2020

[![Azure Flash News: Watch Episode](https://img.youtube.com/vi/eN30WfGWVu4/0.jpg)](https://youtu.be/eN30WfGWVu4 "Azure Flash News: Episode 88")


## Contacts
* Rick Weyenberg  email: rickwey@microsoft.com twitter: [@codeboarder](https://www.twitter.com/codeboarder)
* Mark Garner email: mgarner@microsoft.com twitter: [@mgarner](https://www.twitter.com/mgarner)
* website: [www.azureflashnews.com](http://www.azureflashnews.com)
* twitter: [@azureflashnews](https://www.twitter.com/azureflashnews)
* iTunes: [aka.ms/afn-iTunes](https://aka.ms/afn-iTunes)
* Google Play: [aka.ms/afn-google](https://aka.ms/afn-google)
* Stitcher: [aka.ms/afn-stitcher](https://aka.ms/afn-stitcher)
* Youtube: [Azure Flash News Channel on YouTube](https://www.youtube.com/channel/UCV6U_D4q7OxQaf0rFfEb6fQ)

## Plan migration of physical servers using Azure Migrate

https://azure.microsoft.com/en-us/blog/plan-migration-of-physical-servers-using-azure-migrate/

Previously, Azure Migrate: Server Assessment only supported VMware and Hyper-V virtual machine assessments for migration to Azure. At Ignite 2019, we added physical server support for assessment features like Azure suitability analysis, migration cost planning, performance-based rightsizing, and application dependency analysis. You can now plan at-scale, assessing up to 35K physical servers in one Azure Migrate project. If you use VMware or Hyper-V as well, you can discover and assess both physical and virtual servers in the same project. You can create groups of servers, assess by group and refine the groups further using application dependency information.

## ACR built-in audit policies for Azure Policy is now in preview

https://azure.microsoft.com/en-us/updates/acr-builtin-audit-policies-for-azure-policy-is-now-in-preview/

We are pleased to announce the public preview of Azure Container Registry support for creation of built-in audit policies for Azure Policy.  Once the built-in audit policy is available for the security control, the assessment results can be surfaced through Azure Policy’s Compliance feature.

The following 3 built in policies are planned:
- Network: Provide an audit policy that verifies if Private Link is being used
- Network: Provide an audit policy that checks if the firewall is enabled/ IP-based restrictions are applied
- Data transfer: Provide an audit policy to verify if customer-managed key is used

## Managed keys for Azure Container Registry is now in preview

https://azure.microsoft.com/en-us/updates/managed-keys-for-azure-container-registry-is-now-in-preview/

We are excited to announce the public preview of managed keys for Azure Container Registry. This capability enables customers to bring their own encryption key for Azure Container Registry. By using their own key stored in an Azure Vault to encrypt their images and artifacts, customer are better able to adhere to internal compliance regulations.

## Virtual Network NAT now generally available

https://azure.microsoft.com/en-us/updates/virtual-network-nat-now-generally-available/

Azure Virtual Network NAT (network address translation) simplifies outbound-only Internet connectivity for virtual networks. NAT can be configured for one or more subnets of a virtual network and provides on-demand connectivity for virtual machines.

Virtual Networks NAT is being released into general availability (GA) and provides the following capabilities: 
- On-demand outbound to Internet connectivity without pre-allocation 
- Fully managed and highly resilient 
- One or more static public IP addresses for scale 
- Configurable idle timeout 
- TCP reset for unrecognized connections 
- Multi-dimensional metrics and alerts in Azure Monitor 
- Optional zone isolation for availability zones 
- Virtual Network NAT is now available in all Azure public cloud regions.

## Thanks
Produced by Emily Mackmiller

MTC Facility