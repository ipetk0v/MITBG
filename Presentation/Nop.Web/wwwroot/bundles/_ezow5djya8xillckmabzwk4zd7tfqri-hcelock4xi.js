var Admin = Admin || {};

Admin.Navigation = (function () {
    var buildMap = function () {
        var map = {};
        
        var linkElements = $("a.menu-item-link");

        linkElements.each(function () {
            var parents = $(this).parentsUntil(".sidebar-menu");
            var href;
            var title;
            var parent;
            var grandParent;
            var greatGrandParent;
            switch (parents.length) {
                // items in level one
                case 1:
                    {
                        href = $(parents).find("a").attr("href");
                        title = $(parents).find("a").find("span").html();
                        map[href] = { title: title, link: href, parent: null, grandParent: null };

                        break;
                    }
                    // items in level two, these items have parent but have not grand parent
                case 3:
                    {
                        href = $(parents).eq(0).find("a").attr("href");
                        title = $(parents).eq(0).find("a").find("span").html();
                        parent = $(parents).eq(2).find("a").find("span").html();
                        map[href] = { title: title, link: href, parent: parent, grandParent: null };

                        break;
                    }
                    // items in level three, these items have both parent and grand parent
                case 5:
                    {
                        href = $(parents).eq(0).find("a").attr("href");
                        title = $(parents).eq(0).find("a").find("span").html();
                        parent = $(parents).eq(2).find("a").find("span").html();
                        grandParent = $(parents).eq(4).find("a").find("span").html();
                        map[href] = { title: title, link: href, parent: parent, grandParent: grandParent };
                        break;
                    }
                case 7:
                    {
                        href = $(parents).eq(0).find("a").attr("href");
                        title = $(parents).eq(0).find("a").find("span").html();
                        parent = $(parents).eq(2).find("a").find("span").html();
                        grandParent = $(parents).eq(4).find("a").find("span").html();
                        greatGrandParent = $(parents).eq(6).find("a").find("span").html();
                        map[href] = { title: title, link: href, parent: parent, grandParent: grandParent, greatGrandParent: greatGrandParent };
                        break;
                    }
                default: break;
            }
        });

        return map;
    };

    var map;
   
    var init = function () {
        map = buildMap();
    };
    var events = {};
    return {
        enumerate: function (callback) {
            for (var url in map) {
                var node = map[url];
                callback.call(node, node);
            }
        },
        open: function (url) {
            if (events["open"]) {
                var event = $.Event("open", { url: url });
                events["open"].fire(event);
                if (event.isDefaultPrevented())
                    return;
            }
            window.location.href = url;
        },
        
        initOnce: function () {
            if (!map)
                init();
        },
        init: init
    };
})();

var Admin = Admin || {};

Admin.Search = (function () {
    var itemTemplate = function (data) {
        var path = function () {
            if (data.greatGrandParent) {
                return $("<p/>").text([data.greatGrandParent, data.grandParent, data.parent].join(" > ")).html();
            }
            else if (data.grandParent) {
                return $("<p/>").text([data.grandParent, data.parent].join(" > ")).html();
            }
            else {
                return data.parent;
            }
        };
        var result = '<div id="user-selection">' +
            "<h5>" + data.title + "</h5>";
        if (path()) {
            result = result + path();
        }
        result = result + "</div>";
        return result;
    };

    var substringMatcher = function (enumerate) {
        var byRateAndTitle = function (a, b) {
            if (a.rate < b.rate)
                return 1;
            if (a.rate > b.rate)
                return -1;
            if (a.title < b.title)
                return -1;
            if (a.title > b.title)
                return 1;
            return 0;
        };

        return function findMatches(q, cb) {

            var matches = [];

            var substrRegex = new RegExp(q, "i");
            var helpRegex = new RegExp('nop-templates.com/(.*?)-for-nopcommerce-documentation', 'i');
            var manageThemeResourcesRegex = new RegExp('/admin/(.*?)ThemeAdmin/ManageResources', 'i');

            enumerate(function (item) {

                var rate = 0;

                if (substrRegex.test(item.title)) {
                    rate += 10;
                } else if (substrRegex.test(item.parent)) {
                    rate += 5;
                } else if (substrRegex.test(item.grandParent)) {
                    rate += 3;
                } else if (substrRegex.test(item.greatGrandParent)) {
                    rate += 1;
                }

                if (manageThemeResourcesRegex.test(item.link)) {
                    rate -= 1;
                }

                if (helpRegex.test(item.link)) {
                    rate -= 2;
                }

                if(rate > 0) {
                    item.rate = rate;
                    matches.push(item);
                }
            });

            matches.sort(byRateAndTitle);
            cb(matches);
        };
    };

    return {
        init: function () {
            Admin.Navigation.initOnce();

            var $input = $(".admin-search-box");
            $input.blur(function (e) { e.preventDefault(); e.stopPropagation(); });
            $input.typeahead({ minLength: 2, highlight: true, hint: false },
            {
                name: "pages",
                displayKey: "name",
                templates: {
                    empty: [
                            '<div class="empty-message">',
                              "NO RESULTS",
                            "</div>"
                    ].join("\n"),
                    suggestion: itemTemplate
                },
                source: substringMatcher(Admin.Navigation.enumerate),
                limit: 10
            });

            var navigateTo = function (item) {
                $input.typeahead("val", "");
                Admin.Navigation.open(item.link);
            };

            $input.on("typeahead:selected", function (e, item) {
                navigateTo(item);
            });
        }
    };
})();
