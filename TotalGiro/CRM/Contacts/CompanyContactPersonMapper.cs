using B4F.TotalGiro.Dal;
using B4F.TotalGiro.CRM.Contacts;
using System.Collections.Generic;
using NHibernate.Criterion;
using System.Collections;
using B4F.TotalGiro.CRM;

public class CompanyContactPersonMapper
{
    public static void Delete(IDalSession session, ICompanyContactPerson obj)
    {
        session.Delete(obj);
    }

    public static ICompanyContactPerson GetCompanyContactPerson(IDalSession session,
                                                                IContactCompany company,
                                                                IContactPerson person)
    {
        List<ICriterion> expressions = new List<ICriterion>();
        expressions.Add(Expression.Eq("ContactPerson.Key", person.Key));
        expressions.Add(Expression.Eq("Company.Key", company.Key));
        IList companyContactPersons = session.GetList(typeof(CompanyContactPerson), expressions);
        if (companyContactPersons != null && companyContactPersons.Count == 1)
            return (ICompanyContactPerson)companyContactPersons[0];
        else
            return null;
    }
}
