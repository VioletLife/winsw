﻿using System;
using System.Diagnostics;
using NUnit.Framework;
using winsw;

namespace winswTests
{
    using System;
    using WMI;

    [TestFixture]
    public class ServiceDescriptorTests
    {

        private ServiceDescriptor _extendedServiceDescriptor;

        private const string ExpectedWorkingDirectory = @"Z:\Path\SubPath";
        private const string Username = "User";
        private const string Password = "Password";
        private const string Domain = "Domain";
        private const string AllowServiceAccountLogonRight = "true";

        [SetUp]
        public void SetUp()
        {
            const string seedXml = "<service>"
                                   + "<id>service.exe</id>"
                                   + "<name>Service</name>"
                                   + "<description>The service.</description>"
                                   + "<executable>node.exe</executable>"
                                   + "<arguments>My Arguments</arguments>"
                                   + "<logmode>rotate</logmode>"
                                   + "<serviceaccount>"
                                   +   "<domain>" + Domain + "</domain>"
                                   +   "<user>" + Username + "</user>"
                                   +   "<password>" + Password + "</password>"
                                   + "<allowservicelogon>" + AllowServiceAccountLogonRight + "</allowservicelogon>"
                                   + "</serviceaccount>"
                                   + "<workingdirectory>"
                                   + ExpectedWorkingDirectory
                                   + "</workingdirectory>"
                                   + @"<logpath>C:\logs</logpath>"
                                   + "</service>";
            _extendedServiceDescriptor = ServiceDescriptor.FromXML(seedXml);
        }

        [Test]
        public void DefaultStartMode()
        {
            Assert.That(_extendedServiceDescriptor.StartMode, Is.EqualTo(StartMode.Automatic));
        }

        [Test]
        public void IncorrectStartMode()
        {
            const string SeedXml = "<service>"
                                   + "<id>service.exe</id>"
                                   + "<name>Service</name>"
                                   + "<description>The service.</description>"
                                   + "<executable>node.exe</executable>"
                                   + "<arguments>My Arguments</arguments>"
                                   + "<startmode>rotate</startmode>"
                                   + "<logmode>rotate</logmode>"
                                   + "<serviceaccount>"
                                   + "<domain>" + Domain + "</domain>"
                                   + "<user>" + Username + "</user>"
                                   + "<password>" + Password + "</password>"
                                   + "<allowservicelogon>" + AllowServiceAccountLogonRight + "</allowservicelogon>"
                                   + "</serviceaccount>"
                                   + "<workingdirectory>"
                                   + ExpectedWorkingDirectory
                                   + "</workingdirectory>"
                                   + @"<logpath>C:\logs</logpath>"
                                   + "</service>";



            Assert.Throws(Is.InstanceOf<ArgumentException>(), delegate
            {
                 _extendedServiceDescriptor = ServiceDescriptor.FromXML(SeedXml); 
            });
            Assert.That(_extendedServiceDescriptor.StartMode, Is.EqualTo(StartMode.Manual));
        }

        [Test]
        public void ChangedStartMode()
        {
            const string SeedXml = "<service>"
                                   + "<id>service.exe</id>"
                                   + "<name>Service</name>"
                                   + "<description>The service.</description>"
                                   + "<executable>node.exe</executable>"
                                   + "<arguments>My Arguments</arguments>"
                                   + "<startmode>manual</startmode>"
                                   + "<logmode>rotate</logmode>"
                                   + "<serviceaccount>"
                                   + "<domain>" + Domain + "</domain>"
                                   + "<user>" + Username + "</user>"
                                   + "<password>" + Password + "</password>"
                                   + "<allowservicelogon>" + AllowServiceAccountLogonRight + "</allowservicelogon>"
                                   + "</serviceaccount>"
                                   + "<workingdirectory>"
                                   + ExpectedWorkingDirectory
                                   + "</workingdirectory>"
                                   + @"<logpath>C:\logs</logpath>"
                                   + "</service>";

            _extendedServiceDescriptor = ServiceDescriptor.FromXML(SeedXml);
            Assert.That(_extendedServiceDescriptor.StartMode, Is.EqualTo(StartMode.Manual));
        }
        [Test]
        public void VerifyWorkingDirectory()
        {
            Debug.WriteLine("_extendedServiceDescriptor.WorkingDirectory :: " + _extendedServiceDescriptor.WorkingDirectory);
            Assert.That(_extendedServiceDescriptor.WorkingDirectory, Is.EqualTo(ExpectedWorkingDirectory));
        }

        [Test]
        public void VerifyServiceLogonRight()
        {
            Assert.That(_extendedServiceDescriptor.AllowServiceAcountLogonRight, Is.EqualTo(true));
        }

        [Test]
        public void VerifyUsername()
        {
            Debug.WriteLine("_extendedServiceDescriptor.WorkingDirectory :: " + _extendedServiceDescriptor.WorkingDirectory);
            Assert.That(_extendedServiceDescriptor.ServiceAccountUser, Is.EqualTo(Domain + "\\" + Username));
        }

        [Test]
        public void VerifyPassword()
        {
            Debug.WriteLine("_extendedServiceDescriptor.WorkingDirectory :: " + _extendedServiceDescriptor.WorkingDirectory);
            Assert.That(_extendedServiceDescriptor.ServiceAccountPassword, Is.EqualTo(Password));
        }

        [Test]
        public void Priority()
        {
            var sd = ServiceDescriptor.FromXML("<service><id>test</id><priority>normal</priority></service>");
            Assert.That(sd.Priority, Is.EqualTo(ProcessPriorityClass.Normal));

            sd = ServiceDescriptor.FromXML("<service><id>test</id><priority>idle</priority></service>");
            Assert.That(sd.Priority, Is.EqualTo(ProcessPriorityClass.Idle));

            sd = ServiceDescriptor.FromXML("<service><id>test</id></service>");
            Assert.That(sd.Priority, Is.EqualTo(ProcessPriorityClass.Normal));
        }

        [Test]
        public void StopParentProcessFirstIsFalseByDefault()
        {
            Assert.False(_extendedServiceDescriptor.StopParentProcessFirst);
        }

        [Test]
        public void CanParseStopParentProcessFirst()
        {
            const string seedXml =   "<service>"
                                   +    "<stopparentprocessfirst>true</stopparentprocessfirst>"
                                   + "</service>";
            var serviceDescriptor = ServiceDescriptor.FromXML(seedXml);

            Assert.True(serviceDescriptor.StopParentProcessFirst);
        }

        [Test]
        public void CanParseStopTimeout()
        {
            const string seedXml =   "<service>"
                                   +    "<stoptimeout>60sec</stoptimeout>"
                                   + "</service>";
            var serviceDescriptor = ServiceDescriptor.FromXML(seedXml);

            Assert.That(serviceDescriptor.StopTimeout, Is.EqualTo(TimeSpan.FromSeconds(60)));
        }

        [Test]
        public void CanParseStopTimeoutFromMinutes()
        {
            const string seedXml =   "<service>"
                                   +    "<stoptimeout>10min</stoptimeout>"
                                   + "</service>";
            var serviceDescriptor = ServiceDescriptor.FromXML(seedXml);

            Assert.That(serviceDescriptor.StopTimeout, Is.EqualTo(TimeSpan.FromMinutes(10)));
        }   
        
        [Test]
        public void LogModeRollBySize()
        {
            const string seedXml =   "<service>"
                                   + "<logpath>c:\\</logpath>"
                                   + "<log mode=\"roll-by-size\">"
                                   +    "<sizeThreshold>112</sizeThreshold>"
                                   +    "<keepFiles>113</keepFiles>"
                                   + "</log>"
                                   + "</service>";
            
            var serviceDescriptor = ServiceDescriptor.FromXML(seedXml);
            serviceDescriptor.BaseName = "service";

            var logHandler = serviceDescriptor.LogHandler as SizeBasedRollingLogAppender;
            Assert.NotNull(logHandler);
            Assert.That(logHandler.SizeTheshold, Is.EqualTo(112 * 1024));
            Assert.That(logHandler.FilesToKeep, Is.EqualTo(113));
        }

        [Test]
        public void LogModeRollByTime()
        {
            const string seedXml = "<service>"
                                   + "<logpath>c:\\</logpath>"
                                   + "<log mode=\"roll-by-time\">"
                                   +    "<period>7</period>"
                                   +    "<pattern>log pattern</pattern>"
                                   + "</log>"
                                   + "</service>";

            var serviceDescriptor = ServiceDescriptor.FromXML(seedXml);
            serviceDescriptor.BaseName = "service";

            var logHandler = serviceDescriptor.LogHandler as TimeBasedRollingLogAppender;
            Assert.NotNull(logHandler);
            Assert.That(logHandler.Period, Is.EqualTo(7));
            Assert.That(logHandler.Pattern, Is.EqualTo("log pattern"));
        }

        [Test]
        public void VerifyServiceLogonRightGraceful()
        {
            const string seedXml="<service>"
                                   + "<serviceaccount>"
                                   +   "<domain>" + Domain + "</domain>"
                                   +   "<user>" + Username + "</user>"
                                   +   "<password>" + Password + "</password>"
                                   + "<allowservicelogon>true1</allowservicelogon>"
                                   +  "</serviceaccount>"
                                   + "</service>";
            var serviceDescriptor = ServiceDescriptor.FromXML(seedXml);
            Assert.That(serviceDescriptor.AllowServiceAcountLogonRight, Is.EqualTo(false));
        }
        [Test]
        public void VerifyServiceLogonRightOmitted()
        {
            const string seedXml = "<service>"
                                   + "<serviceaccount>"
                                   + "<domain>" + Domain + "</domain>"
                                   + "<user>" + Username + "</user>"
                                   + "<password>" + Password + "</password>"
                                   + "</serviceaccount>"
                                   + "</service>";
            var serviceDescriptor = ServiceDescriptor.FromXML(seedXml);
            Assert.That(serviceDescriptor.AllowServiceAcountLogonRight, Is.EqualTo(false));
        }
    }
}
