// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.PerformanceTests.TestDomain;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  [TestFixture]
  public class LinqTest : DatabaseTest
  {
    [Test]
    public void WithTinyResultSet ()
    {
      Func<IQueryable<Client>> queryGenerator = () => (from c in QueryFactory.CreateLinqQuery<Client>() select c);
      var linqHelper = new LinqPerformanceTestHelper<Client> (queryGenerator);

      var message = "Simple query with tiny (1) result set ";
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM)", linqHelper.GenerateQueryModel);
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM+SQL)", linqHelper.GenerateQueryModelAndSQL);
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM+SQL+IQ)", linqHelper.GenerateQueryModelAndSQLAndIQuery);
      PerformanceTestHelper.TimeAndOutput (2000, message + "(QM+SQL+IQ+Execute)", linqHelper.GenerateAndExecuteQueryDBOnly);
      PerformanceTestHelper.TimeAndOutput (1000, message + "(QM+SQL+IQ+Execute+re-store)", linqHelper.GenerateAndExecuteQuery);
    }

    [Test]
    public void WithCustomProjectionAndTinyResultSet ()
    {
      var linqHelper = CreateLinqPerformanceTestHelper (() => from c in QueryFactory.CreateLinqQuery<Client>() select new { c.ID, c.Name });

      var message = "Custom projection query with tiny (1) result set ";
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM)", linqHelper.GenerateQueryModel);
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM+SQL)", linqHelper.GenerateQueryModelAndSQL);
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM+SQL+IQ)", linqHelper.GenerateQueryModelAndSQLAndIQuery);
      //PerformanceTestHelper.TimeAndOutput (2000, message +"(QM+SQL+IQ+Execute)", linqHelper.GenerateAndExecuteQueryDBOnly);
      PerformanceTestHelper.TimeAndOutput (1000, message + "(QM+SQL+IQ+Execute+re-store)", linqHelper.GenerateAndExecuteQuery);
    }

    [Test]
    public void WithSmallResultSet ()
    {
      Func<IQueryable<Company>> queryGenerator = () => (from c in QueryFactory.CreateLinqQuery<Company>() select c);
      var linqHelper = new LinqPerformanceTestHelper<Company> (queryGenerator);

      var message = "Simple query with small (200) result set ";
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM)", linqHelper.GenerateQueryModel);
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM+SQL)", linqHelper.GenerateQueryModelAndSQL);
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM+SQL+IQ)", linqHelper.GenerateQueryModelAndSQLAndIQuery);
      PerformanceTestHelper.TimeAndOutput (2000, message + "(QM+SQL+IQ+Execute)", linqHelper.GenerateAndExecuteQueryDBOnly);
      PerformanceTestHelper.TimeAndOutput (1000, message + "(QM+SQL+IQ+Execute+re-store)", linqHelper.GenerateAndExecuteQuery);
    }

    [Test]
    public void WithCustomProjectionAndSmallResultSet ()
    {
      var linqHelper = CreateLinqPerformanceTestHelper (() => from c in QueryFactory.CreateLinqQuery<Company>() select new { c.ID, c.Name });

      var message = "Custom projection query with small (200) result set ";
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM)", linqHelper.GenerateQueryModel);
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM+SQL)", linqHelper.GenerateQueryModelAndSQL);
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM+SQL+IQ)", linqHelper.GenerateQueryModelAndSQLAndIQuery);
      //PerformanceTestHelper.TimeAndOutput (2000, message + "(QM+SQL+IQ+Execute)", linqHelper.GenerateAndExecuteQueryDBOnly);
      PerformanceTestHelper.TimeAndOutput (1000, message + "(QM+SQL+IQ+Execute+re-store)", linqHelper.GenerateAndExecuteQuery);
    }

    [Test]
    public void WithLargeResultSet ()
    {
      Func<IQueryable<Person>> queryGenerator = () => (from p in QueryFactory.CreateLinqQuery<Person> () select p);
      var linqHelper = new LinqPerformanceTestHelper<Person> (queryGenerator);

      var message = "Simple query with large (2000) result set ";
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM)", linqHelper.GenerateQueryModel);
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM+SQL)", linqHelper.GenerateQueryModelAndSQL);
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM+SQL+IQ)", linqHelper.GenerateQueryModelAndSQLAndIQuery);
      PerformanceTestHelper.TimeAndOutput (100, message + "(QM+SQL+IQ+Execute)", linqHelper.GenerateAndExecuteQueryDBOnly);
      PerformanceTestHelper.TimeAndOutput (100, message + "(QM+SQL+IQ+Execute+re-store)", linqHelper.GenerateAndExecuteQuery);
    }
    
    [Test]
    public void WithCustomProjectionAndLargeResultSet ()
    {
      var linqHelper = CreateLinqPerformanceTestHelper (() => from c in QueryFactory.CreateLinqQuery<Person>() select new { c.ID, c.LastName });

      var message = "Custom projection query with large (2000) result set ";
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM)", linqHelper.GenerateQueryModel);
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM+SQL)", linqHelper.GenerateQueryModelAndSQL);
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM+SQL+IQ)", linqHelper.GenerateQueryModelAndSQLAndIQuery);
      //PerformanceTestHelper.TimeAndOutput (100, message + "(QM+SQL+IQ+Execute)", linqHelper.GenerateAndExecuteQueryDBOnly);
      PerformanceTestHelper.TimeAndOutput (100, message + "(QM+SQL+IQ+Execute+re-store)", linqHelper.GenerateAndExecuteQuery);
    }

    [Test]
    public void WithComplexQuery_Subqueries ()
    {
      var query = (from p in QueryFactory.CreateLinqQuery<Person> () 
                   where (from c in QueryFactory.CreateLinqQuery<Client> () select c).Contains(p.Client) 
                        && p.FirstName == (from sp in QueryFactory.CreateLinqQuery<Person> () select sp.FirstName).First()
                   select p).Distinct ();
      Func<IQueryable<Person>> queryGenerator = () => (query);
      var linqHelper = new LinqPerformanceTestHelper<Person> (
          queryGenerator);

      var message = "Complex query with subqueries ";
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM)", linqHelper.GenerateQueryModel);
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM+SQL)", linqHelper.GenerateQueryModelAndSQL);
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM+SQL+IQ)", linqHelper.GenerateQueryModelAndSQLAndIQuery);
      PerformanceTestHelper.TimeAndOutput (1000, message + "(QM+SQL+IQ+Execute)", linqHelper.GenerateAndExecuteQueryDBOnly);
      PerformanceTestHelper.TimeAndOutput (500, message + "(QM+SQL+IQ+Execute+re-store)", linqHelper.GenerateAndExecuteQuery);
    }

    [Test]
    public void WithComplexQuery_SubqueriesInSecondFromClauseAndMultiplyOrderByClauses ()
    {
      var query =  (from c in QueryFactory.CreateLinqQuery<Client> ()
                   from p in (from sp in QueryFactory.CreateLinqQuery<Person>() where sp.Client==c orderby sp.FirstName orderby sp.LastName select sp)
                   from co in (from sco in QueryFactory.CreateLinqQuery<Company>() where sco.Client==c orderby sco.Name select sco)
                   select p).Distinct();
      Func<IQueryable<Person>> queryGenerator = () => (query);
      var linqHelper = new LinqPerformanceTestHelper<Person> (
          queryGenerator);

      var message = "Complex query with subqueries in second from clause and multiply order by clauses ";
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM)", linqHelper.GenerateQueryModel);
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM+SQL)", linqHelper.GenerateQueryModelAndSQL);
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM+SQL+IQ)", linqHelper.GenerateQueryModelAndSQLAndIQuery);
      PerformanceTestHelper.TimeAndOutput (1000, message + "(QM+SQL+IQ+Execute)", linqHelper.GenerateAndExecuteQueryDBOnly);
      PerformanceTestHelper.TimeAndOutput (500, message + "(QM+SQL+IQ+Execute+re-store)", linqHelper.GenerateAndExecuteQuery);
    }

    [Test]
    public void WithComplexQuery_JoinsAndSubquery ()
    {
      var query = (from c in QueryFactory.CreateLinqQuery<Client> () 
                    join p in QueryFactory.CreateLinqQuery<Person> () on c equals p.Client
                    join co in QueryFactory.CreateLinqQuery<Company> () on c equals co.Client 
                  where p.FirstName== ((from sp in QueryFactory.CreateLinqQuery<Person> () select sp.FirstName).First())
                  select co).Distinct();
      Func<IQueryable<Company>> queryGenerator = () => (query);
      var linqHelper = new LinqPerformanceTestHelper<Company> (
          queryGenerator);

      var message = "Complex query with joins and subquery ";
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM)", linqHelper.GenerateQueryModel);
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM+SQL)", linqHelper.GenerateQueryModelAndSQL);
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM+SQL+IQ)", linqHelper.GenerateQueryModelAndSQLAndIQuery);
      PerformanceTestHelper.TimeAndOutput (1000, message + "(QM+SQL+IQ+Execute)", linqHelper.GenerateAndExecuteQueryDBOnly);
      PerformanceTestHelper.TimeAndOutput (500, message + "(QM+SQL+IQ+Execute+re-store)", linqHelper.GenerateAndExecuteQuery);
    }

    [Test]
    public void WithComplexQuery_GroupBy ()
    {
      var query = (from p in QueryFactory.CreateLinqQuery<Person> () group p by p.FirstName into gp from grp in gp select grp);
      Func<IQueryable<Person>> queryGenerator = () => (query);
      var linqHelper = new LinqPerformanceTestHelper<Person> (queryGenerator);

      var message = "Complex query with group by ";
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM)", linqHelper.GenerateQueryModel);
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM+SQL)", linqHelper.GenerateQueryModelAndSQL);
      PerformanceTestHelper.TimeAndOutput (10000, message + "(QM+SQL+IQ)", linqHelper.GenerateQueryModelAndSQLAndIQuery);
      PerformanceTestHelper.TimeAndOutput (1000, message + "(QM+SQL+IQ+Execute)", linqHelper.GenerateAndExecuteQueryDBOnly);
      PerformanceTestHelper.TimeAndOutput (500, message + "(QM+SQL+IQ+Execute+re-store)", linqHelper.GenerateAndExecuteQuery);
    }

    private LinqPerformanceTestHelper<T> CreateLinqPerformanceTestHelper<T> (Func<IQueryable<T>> queryGenerator)
    {
      return new LinqPerformanceTestHelper<T> (queryGenerator);
    }
  }
}