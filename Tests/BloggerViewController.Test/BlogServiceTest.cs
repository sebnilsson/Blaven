using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BloggerViewController.Configuration;
using BloggerViewController.Data;

namespace BloggerViewController.Test {
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class BlogServiceTest {
        public BlogServiceTest() {
            
        }

        [TestMethod]
        public void TestMethod1() {
            var blogStore = new MemoryBlogStore();
            var settings = new[] { new BloggerSetting() };

            var blogService = new BlogService(blogStore, settings);
        }
    }
}
