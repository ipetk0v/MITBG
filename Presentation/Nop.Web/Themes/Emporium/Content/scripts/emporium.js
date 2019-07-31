(function ($) {

    $(document).ready(function () {

        var closeMenuSelector = '.close-menu > span.close-menu-btn';
        var closeSideMenu = '.close-side-menu-btn';
        var flyoutCartScrollbarSelector = '#flyout-cart .flyout-cart-scroll-area';

        var dependencies = [
            {
                module: "header",
                dependencies: ["attachDetach", "overlay", "perfectScrollbar"]
            },
            {
                module: "menu",
                dependencies: ["perfectScrollbar"]
            }
        ];

        var themeSettings = {
            overlay: {
                overlayElementSelector: '.overlayOffCanvas',
                overlayClass: 'show',
                noPageScrollClass: 'scrollYRemove'
            },
            productQuantity: {
                quantityInputSelector: '.qty-input, .productQuantityTextBox',
                incrementSelectors: '.plus',
                decrementSelectors: '.minus'
            },
            flyoutCart: {
                flyoutCartSelector: '#flyout-cart',
                flyoutCartScrollbarSelector: flyoutCartScrollbarSelector + ' .items',
                removeItemSelector: '#flyout-cart .remove-item'
            },
            header: {
                activeClass: 'open',
                modules: [
                    {
                        opener: '.header-links .opener',
                        content: '.profile-menu-box',
                        preventClicking: true
                    },
	                {
	                    content: '.flyout-cart',
	                    overlay: false,
	                    scrollbar: flyoutCartScrollbarSelector + ' .items'
	                }
                ]
            },
            goToTop: {
                animation: {
                    type: 'slide',      // Fade, slide, none
                    speed: 500            // Animation in speed (ms)
                }
            },
            toggle: {
                blocks: [
                    {
                        opener: '.write-review .title',
                        content: '.write-review-collapse',
                        activeClassOpener: 'open',
                        animation: {
                            type: 'slide',
                            speed: 'slow'
                        }
                    },
                    {
                        opener: '.comment-form-btn',
                        content: '.new-comment form',
                        activeClassOpener: 'open',
                        animation: {
                            type: 'slide',
                            speed: 'slow'
                        }
                    },
                    {
                        opener: '.cart-collaterals > div > .title',
                        content: '.cart-collaterals > div > .list',
                        activeClassOpener: 'open',
                        animation: {
                            type: 'slide',
                            speed: 'slow'
                        }
                    },
                    {
                        opener: '.add-button .btn-holder',
                        content: '.add-button .enter-address',
                        activeClassOpener: 'open',
                        animation: {
                            type: 'slide',
                            speed: 'slow'
                        }
                    }
                ]
            },
            equalizer: {
                blocks: [
                    {
                        selector: '.address-list > div',
                        property: 'height',
                        measurementFunction: 'innerHeight'
                    },
                    {
                        selector: '.address-grid > div',
                        property: 'min-height',
                        measurementFunction: 'innerHeight'
                    },
                    {
                        selector: '.order-item',
                        property: 'height',
                        measurementFunction: 'innerHeight'
                    },
                    {
                        selector: '.request-item',
                        property: 'height',
                        measurementFunction: 'innerHeight'
                    },
                    {
                        selector: '.news-item',
                        property: 'height',
                        measurementFunction: 'innerHeight'
                    },
                    {
                        selector: '.blog-posts .post',
                        property: 'height',
                        measurementFunction: 'innerHeight'
                    },
                    {
                        selector: '.order-details-area > div',
                        property: 'height',
                        measurementFunction: 'innerHeight'
                    },
                    {
                        selector: '.order-review-data > div',
                        property: 'height',
                        measurementFunction: 'innerHeight'
                    },
                    {
                        selector: '.shipment-details-area > div',
                        property: 'height',
                        measurementFunction: 'innerHeight'
                    },
                    {
                        selector: '.rich-blog-homepage .blog-post',
                        property: 'height',
                        measurementFunction: 'innerHeight'
                    }
                ]
            },
            responsive: [
                {
                    breakpoint: 1281,
                    settings: {
                        attachDetach: {
                            blocks: [
                                {
                                    elementToAttach: ".product-details-page-body .mobile-name-holder",
                                    content: ".product-details-page-body .breadcrumb"
                                },
                                {
                                    elementToAttach: ".product-details-page-body .mobile-name-holder",
                                    content: ".product-details-page-body .product-name"
                                },
                                {
                                    elementToAttach: ".product-details-page-body .attributes-wrapper",
                                    content: ".product-details-page-body .product-prices-box"
                                },
                                {
                                    elementToAttach: ".product-details-page .mobile-prev-next-holder",
                                    content: ".product-details-page .prev-next-holder"
                                }
                            ]
                        },
                        header: {
                            activeClass: 'open',
                            modules: [
                                {
                                    opener: '.header-links .opener',
                                    content: '.profile-menu-box',
                                    preventClicking: true
                                },
                                {
                                    content: '.flyout-cart',
                                    overlay: false,
                                    scrollbar: flyoutCartScrollbarSelector + ' .items'
                                }
                            ]
                        },
                        equalizer: {
                            blocks: [
                                {
                                    selector: '.address-list > div',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.address-grid > div',
                                    property: 'min-height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.order-item',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.request-item',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.news-item',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.blog-posts .post',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.order-details-area > div',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.order-review-data > div',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.shipment-details-area > div',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.rich-blog-homepage .blog-post',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                }
                            ]
                        }
                    }
                },
                {
                    breakpoint: 1025,
                    settings: {
                        menu: {
                            closeMenuSelector: closeMenuSelector,
                            sublistIndent: {
                                enabled: true
                            }
                        },
                        attachDetach: {
                            blocks: [
                                {
                                    content: '#topcartlink .ico-cart',
                                    elementToAttach: '.responsive-nav-wrapper .shopping-cart-link'
                                },
                                {
                                    content: '.header-selectors-wrapper',
                                    elementToAttach: '.mobile-menu-items'
                                },
                                {
                                    content: '.ico-compare',
                                    elementToAttach: '.header-menu'
                                },
                                {
                                    content: '.ico-wishlist',
                                    elementToAttach: '.header-menu'
                                },
                                {
                                    elementToAttach: ".product-details-page-body .mobile-name-holder",
                                    content: ".product-details-page-body .breadcrumb"
                                },
                                {
                                    elementToAttach: ".product-details-page-body .mobile-name-holder",
                                    content: ".product-details-page-body .product-name"
                                },
                                {
                                    elementToAttach: ".product-details-page-body .attributes-wrapper",
                                    content: ".product-details-page-body .product-prices-box"
                                },
                                {
                                    elementToAttach: ".product-details-page .mobile-prev-next-holder",
                                    content: ".product-details-page .prev-next-holder"
                                }
                            ]
                        },
                        header: {
                            activeClass: 'open',
                            modules: [
                                {
                                    opener: '.search-wrap > span',
                                    content: '.store-search-box',
                                    closer: '.store-search-box ' + closeSideMenu,
                                    elementToAttach: '.master-wrapper-page',
                                    overlay: true
                                },
                                {
                                    opener: '.personal-button > span',
                                    closer: '.profile-menu-box ' + closeSideMenu,
                                    content: '.profile-menu-box',
                                    elementToAttach: '.header',
                                    overlay: true
                                },
                                {
                                    opener: '.shopping-cart-link',
                                    closer: '.mobile-flyout-wrapper ' + closeSideMenu,
                                    content: '.mobile-flyout-wrapper',
                                    preventClicking: true,
                                    overlay: true,
                                    scrollbar: flyoutCartScrollbarSelector
                                },
                                {
                                    opener: '.responsive-nav-wrapper .menu-title > span',
                                    closer: closeMenuSelector,
                                    content: '.header-menu',
                                    overlay: true,
                                    scrollbar: true
                                },
                                {
                                    elementToAttach: '.master-wrapper-content',
                                    opener: '.filters-button-wrapper .filters-button',
                                    closer: '.nopAjaxFilters7Spikes ' + closeSideMenu,
                                    content: '.nopAjaxFilters7Spikes',
                                    overlay: true,
                                    scrollbar: true
                                }
                            ]
                        },
                        filters: true,
                        toggle: {
                            blocks: [
                                {
                                    opener: '.block > .title',
                                    content: '.block > .listbox',
                                    animation: {
                                        type: 'slide'
                                    }
                                },
                                {
                                    opener: '.mobile-selector .selector-title',
                                    content: '.mobile-collapse',
                                    activeClassOpener: 'open',
                                    animation: {
                                        type: 'slide',
                                        speed: 'slow'
                                    }
                                },
                                {
                                    opener: '.write-review .title',
                                    content: '.write-review-collapse',
                                    activeClassOpener: 'open',
                                    animation: {
                                        type: 'slide',
                                        speed: 'slow'
                                    }
                                },
                                {
                                    opener: '.comment-form-btn',
                                    content: '.new-comment form',
                                    activeClassOpener: 'open',
                                    animation: {
                                        type: 'slide',
                                        speed: 'slow'
                                    }
                                },
                                {
                                    opener: '.cart-collaterals > div > .title',
                                    content: '.cart-collaterals > div > .list',
                                    activeClassOpener: 'open',
                                    animation: {
                                        type: 'slide',
                                        speed: 'slow'
                                    }
                                },
                                {
                                    opener: '.add-button .btn-holder',
                                    content: '.add-button .enter-address',
                                    activeClassOpener: 'open',
                                    animation: {
                                        type: 'slide',
                                        speed: 'slow'
                                    }
                                }
                            ]
                        },
                        equalizer: {
                            blocks: [
                                {
                                    selector: '.cart tr .product'
                                },
                                {
                                    selector: '.data-table .product'
                                },
                                {
                                    selector: '.address-list > div',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.address-grid > div',
                                    property: 'min-height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.order-item',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.request-item',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.news-item',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.blog-posts .post',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.order-details-area > div',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.order-review-data > div',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.shipment-details-area > div',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.rich-blog-homepage .blog-post',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                }
                            ]
                        }
                    }
                },
                {
                    breakpoint: 769,
                    settings: {
                        menu: {
                            closeMenuSelector: closeMenuSelector,
                            sublistIndent: {
                                enabled: true
                            }
                        },
                        attachDetach: {
                            blocks: [
                                {
                                    content: '#topcartlink .ico-cart',
                                    elementToAttach: '.responsive-nav-wrapper .shopping-cart-link'
                                },
                                {
                                    content: '.header-selectors-wrapper',
                                    elementToAttach: '.mobile-menu-items'
                                },
                                {
                                    content: '.ico-compare',
                                    elementToAttach: '.header-menu'
                                },
                                {
                                    content: '.ico-wishlist',
                                    elementToAttach: '.header-menu'
                                },
                                {
                                    elementToAttach: ".product-details-page-body .mobile-name-holder",
                                    content: ".product-details-page-body .breadcrumb"
                                },
                                {
                                    elementToAttach: ".product-details-page-body .mobile-name-holder",
                                    content: ".product-details-page-body .product-name"
                                },
                                {
                                    elementToAttach: ".product-details-page-body .attributes-wrapper",
                                    content: ".product-details-page-body .product-prices-box"
                                },
                                {
                                    elementToAttach: ".product-details-page .mobile-prev-next-holder",
                                    content: ".product-details-page .prev-next-holder"
                                }
                            ]
                        },
                        header: {
                            activeClass: 'open',
                            modules: [
                                {
                                    opener: '.search-wrap > span',
                                    content: '.store-search-box',
                                    closer: '.store-search-box ' + closeSideMenu,
                                    elementToAttach: '.master-wrapper-page',
                                    overlay: true
                                },
                                {
                                    opener: '.personal-button > span',
                                    closer: '.profile-menu-box ' + closeSideMenu,
                                    content: '.profile-menu-box',
                                    elementToAttach: '.header',
                                    overlay: true
                                },
                                {
                                    opener: '.shopping-cart-link',
                                    closer: '.mobile-flyout-wrapper ' + closeSideMenu,
                                    content: '.mobile-flyout-wrapper',
                                    preventClicking: true,
                                    overlay: true,
                                    scrollbar: flyoutCartScrollbarSelector
                                },
                                {
                                    opener: '.responsive-nav-wrapper .menu-title > span',
                                    closer: closeMenuSelector,
                                    content: '.header-menu',
                                    overlay: true,
                                    scrollbar: true
                                },
                                {
                                    elementToAttach: '.master-wrapper-content',
                                    opener: '.filters-button-wrapper .filters-button',
                                    closer: '.nopAjaxFilters7Spikes ' + closeSideMenu,
                                    content: '.nopAjaxFilters7Spikes',
                                    overlay: true,
                                    scrollbar: true
                                }
                            ]
                        },
                        filters: true,
                        toggle: {
                            blocks: [
                                {
                                    opener: '.block > .title',
                                    content: '.block > .listbox',
                                    animation: {
                                        type: 'slide'
                                    }
                                },
                                {
                                    opener: '.mobile-selector .selector-title',
                                    content: '.mobile-collapse',
                                    activeClassOpener: 'open',
                                    animation: {
                                        type: 'slide',
                                        speed: 'slow'
                                    }
                                },
                                {
                                    opener: '.write-review .title',
                                    content: '.write-review-collapse',
                                    activeClassOpener: 'open',
                                    animation: {
                                        type: 'slide',
                                        speed: 'slow'
                                    }
                                },
                                {
                                    opener: '.comment-form-btn',
                                    content: '.new-comment form',
                                    activeClassOpener: 'open',
                                    animation: {
                                        type: 'slide',
                                        speed: 'slow'
                                    }
                                },
                                {
                                    opener: '.cart-collaterals > div > .title',
                                    content: '.cart-collaterals > div > .list',
                                    activeClassOpener: 'open',
                                    animation: {
                                        type: 'slide',
                                        speed: 'slow'
                                    }
                                },
                                {
                                    opener: '.add-button .btn-holder',
                                    content: '.add-button .enter-address',
                                    activeClassOpener: 'open',
                                    animation: {
                                        type: 'slide',
                                        speed: 'slow'
                                    }
                                }
                            ]
                        },
                        equalizer: {
                            blocks: [
                                {
                                    selector: '.cart tr .product'
                                },
                                {
                                    selector: '.data-table .product'
                                },
                                {
                                    selector: '.address-list > div',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.address-grid > div',
                                    property: 'min-height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.order-item',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.request-item',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.news-item',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.blog-posts .post',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.order-details-area > div',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.order-review-data > div',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.shipment-details-area > div',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                },
                                {
                                    selector: '.rich-blog-homepage .blog-post',
                                    property: 'height',
                                    measurementFunction: 'innerHeight'
                                }
                            ]
                        }
                    }
                },
                {
                    breakpoint: 481,
                    settings: {
                        menu: {
                            closeMenuSelector: closeMenuSelector,
                            sublistIndent: {
                                enabled: true
                            }
                        },
                        flyoutCart: {
                            flyoutCartSelector: '#flyout-cart',
                            flyoutCartScrollbarSelector: flyoutCartScrollbarSelector,
                            removeItemSelector: '#flyout-cart .remove-item'
                        },
                        attachDetach: {
                            blocks: [
                                {
                                    content: '#topcartlink .ico-cart',
                                    elementToAttach: '.responsive-nav-wrapper .shopping-cart-link'
                                },
                                {
                                    content: '.header-selectors-wrapper',
                                    elementToAttach: '.mobile-menu-items'
                                },
                                {
                                    content: '.ico-compare',
                                    elementToAttach: '.header-menu'
                                },
                                {
                                    content: '.ico-wishlist',
                                    elementToAttach: '.header-menu'
                                },
                                {
                                    elementToAttach: ".product-details-page-body .mobile-name-holder",
                                    content: ".product-details-page-body .breadcrumb"
                                },
                                {
                                    elementToAttach: ".product-details-page-body .mobile-name-holder",
                                    content: ".product-details-page-body .product-name"
                                },
                                {
                                    elementToAttach: ".product-details-page-body .attributes-wrapper",
                                    content: ".product-details-page-body .product-prices-box"
                                },
                                {
                                    elementToAttach: ".product-details-page .mobile-prev-next-holder",
                                    content: ".product-details-page .prev-next-holder"
                                }
                            ]
                        },
                        header: {
                            activeClass: 'open',
                            modules: [
                                {
                                    opener: '.search-wrap > span',
                                    content: '.store-search-box',
                                    closer: '.store-search-box ' + closeSideMenu,
                                    elementToAttach: '.master-wrapper-page',
                                    overlay: true
                                },
                                {
                                    opener: '.personal-button > span',
                                    closer: '.profile-menu-box ' + closeSideMenu,
                                    content: '.profile-menu-box',
                                    elementToAttach: '.header',
                                    overlay: true
                                },
                                {
                                    opener: '.shopping-cart-link',
                                    closer: '.mobile-flyout-wrapper ' + closeSideMenu,
                                    content: '.mobile-flyout-wrapper',
                                    preventClicking: true,
                                    overlay: true,
                                    scrollbar: flyoutCartScrollbarSelector
                                },
                                {
                                    opener: '.responsive-nav-wrapper .menu-title > span',
                                    closer: closeMenuSelector,
                                    content: '.header-menu',
                                    overlay: true,
                                    scrollbar: true
                                },
                                {
                                    elementToAttach: '.master-wrapper-content',
                                    opener: '.filters-button-wrapper .filters-button',
                                    closer: '.nopAjaxFilters7Spikes ' + closeSideMenu,
                                    content: '.nopAjaxFilters7Spikes',
                                    overlay: true,
                                    scrollbar: true
                                }
                            ]
                        },
                        filters: true,
                        toggle: {
                            blocks: [
                                {
                                    opener: '.block > .title',
                                    content: '.block > .listbox',
                                    animation: {
                                        type: 'slide'
                                    }
                                },
                                {
                                    opener: '.mobile-selector .selector-title',
                                    content: '.mobile-collapse',
                                    activeClassOpener: 'open',
                                    animation: {
                                        type: 'slide',
                                        speed: 'slow'
                                    }
                                },
                                {
                                    opener: '.footer-block > .title',
                                    content: '.footer-block > .list',
                                    animation: {
                                        type: 'slide'
                                    }
                                },
                                {
                                    opener: '.write-review .title',
                                    content: '.write-review-collapse',
                                    activeClassOpener: 'open',
                                    animation: {
                                        type: 'slide',
                                        speed: 'slow'
                                    }
                                },
                                {
                                    opener: '.comment-form-btn',
                                    content: '.new-comment form',
                                    activeClassOpener: 'open',
                                    animation: {
                                        type: 'slide',
                                        speed: 'slow'
                                    }
                                },
                                {
                                    opener: '.cart-collaterals > div > .title',
                                    content: '.cart-collaterals > div > .list',
                                    activeClassOpener: 'open',
                                    animation: {
                                        type: 'slide',
                                        speed: 'slow'
                                    }
                                },
                                {
                                    opener: '.add-button .btn-holder',
                                    content: '.add-button .enter-address',
                                    activeClassOpener: 'open',
                                    animation: {
                                        type: 'slide',
                                        speed: 'slow'
                                    }
                                }
                            ]
                        },
                        equalizer: {
                            blocks: [
                                {
                                    selector: '.cart tr .product'
                                },
                                {
                                    selector: '.data-table .product'
                                }
                            ]
                        }
                    }
                }

            ]
        };

        var theme = new window.sevenSpikesTheme(themeSettings, dependencies, false);

        theme.init();

        groupedButton();
        handleHeaderMenuCategories();
        menuEqualizer();
        accountPageColumns();
        handleClearCartButton();
        updateCartMobiile();
        handleLastActiveCheckoutStep();
        handleHomePageFeaturedProductsCarousel();
        handleHomePageBestsellerProductsCarousel();
        setMaxHeightOnMobile('.header-menu');
        setMaxHeightOnMobile('.mobile-flyout-wrapper .flyout-cart-scroll-area', '.close-side-menu');
        setMaxHeightOnMobile('.header-menu .sublist-wrap');
        handleExpand(".cart-collaterals .deals .message-failure", ".cart-collaterals .deals > .list", ".cart-collaterals > .deals > .title");
        handleExpand(".cart-collaterals .collaterals-shipping .message-failure", ".cart-collaterals .collaterals-shipping > .list", ".cart-collaterals .collaterals-shipping > .title");
        handleExpand(".write-review .field-validation-error", ".write-review-collapse", ".write-review > .title");
        handleExpand(".new-comment .field-validation-error", ".new-comment form", ".comment-form-btn");

        if ($('.profile-menu-box').hasClass('login-form-in-header')) {

            showLoginFormInHeaderPanel();
        }

        $(window).on('resize', function () {
            setMaxHeightOnMobile('.header-menu');
            setMaxHeightOnMobile('.mobile-flyout-wrapper .flyout-cart-scroll-area', '.close-side-menu');
            setMaxHeightOnMobile('.header-menu .sublist-wrap');
            menuEqualizer();
            accountPageColumns();
        });

        $(document).on("quickTabsRefreshedTab", function () {

            handleExpand(".write-review .field-validation-error", ".write-review-collapse", ".write-review > .title");
        });
        
        function groupedButton() {

            if (!$('.product-variant-line').length) {

                return;
            }

            //Sets the number of product varinats in the button
            $(".grouped-btn > span").text($('.product-variant-line').length);

            //Click on the button scrolls to the first variant
            $(".grouped-btn").on('click', function(event) {

                event.preventDefault();

                $('html, body').animate({
                    scrollTop: $('#product-list-start').offset().top
                }, 1000);
            });
        }
        
        function handleHeaderMenuCategories() {
            if ($('.header-menu.categories-in-side-panel').length === 0) {
                return;
            }

            var sideCategoriesList = $('.category-navigation-list-wrapper > .category-navigation-list');

            var topMenuCategoryItems = $('.top-menu li.root-category-items, ' +
                '.mega-menu > li.root-category-items, ' +
                '.mega-menu > li.category-menu-item > .sublist-wrap > .sublist > li.root-category-items');

            var topMenuCategoryItemsCloned = topMenuCategoryItems.clone();

            topMenuCategoryItemsCloned.removeClass('root-category-items').appendTo(sideCategoriesList);
        }

        function menuEqualizer() {

            if ($(window).width() > 1024) {

                sublistHeight();
            }

            //Sets a minimum height to each sublist according to the height of its parent
            function sublistHeight() {

                $('.category-navigation-list .sublist, .category-navigation-list .dropdown .row-wrapper').each(function () {
                    var $this = $(this);
                    var minHeight = $this.parents('ul').outerHeight();

                    $this.css('min-height', minHeight);
                });
            }
        }
        
        function accountPageColumns() {
            var navHeight = $('.block-account-navigation').outerHeight();

            $('.account-page').css('min-height', navHeight);
        }

        function handleClearCartButton() {

            $('.order-summary-content .clear-cart-button').on('click', function (e) {
                e.preventDefault();

                $('.cart [name="removefromcart"]').attr('checked', 'checked');

                $('.order-summary-content .update-cart-button').click();
            });
        }
        
        function updateCartMobiile() {

            $(document).on('removeItemFlyoutCart', function() {

                var windowWidth = $(window).width();

                if ($('.shopping-cart-link .ico-cart').length && windowWidth < 1025) {

                    $('.shopping-cart-link .ico-cart').remove();

                    $(document).trigger('detachElement.theme', ['#topcartlink .ico-cart', '.responsive-nav-wrapper .shopping-cart-link']);
                }
            });
        }

        function handleLastActiveCheckoutStep() {

            // add a class to the last active checkout step. 
            $('.active-step:last').addClass('last');
        }


        function handleExpand(errors, form, opener) {

            if ($(errors).length > 0) {
                $(form).slideDown('slow', function () {

                    $('html, body').animate({
                        scrollTop: $(opener).offset().top
                    }, 300);
                });

                $(opener).addClass("open");
            }
        }

        function showLoginFormInHeaderPanel() {

            var loginUrl = $('.header-links .ico-account').attr('data-loginUrl');

            $.ajax({
                url: loginUrl,
                type: 'GET',
                data: {
                    'isHeaderPanel': true
                }
            }).done(function (data) {
                var dataObj = $(data);
                var loginFields = dataObj.find('.returning-wrapper');

                if (loginFields.length > 0) {

                    $('.profile-menu-box .header-form-holder').html(loginFields);
                }

            }).fail(function () {
                window.location.href = loginUrl;
            });
        }

        function handleHomePageFeaturedProductsCarousel() {
            var featuredProductsCarousel = $('.product-grid.home-page-product-grid .item-grid').not('.jCarouselMainWrapper .product-grid.home-page-product-grid .item-grid');

            if (featuredProductsCarousel.length === 0) {
                return;
            }

            initializeSlickCarousel(featuredProductsCarousel);
        }

        function handleHomePageBestsellerProductsCarousel() {
            var bestsellerProductsCarousel = $('.product-grid.bestsellers .item-grid').not('.jCarouselMainWrapper .product-grid.bestsellers .item-grid');;

            if (bestsellerProductsCarousel.length === 0) {
                return;
            }

            initializeSlickCarousel(bestsellerProductsCarousel);
        }

        function initializeSlickCarousel(productsContainter) {
            productsContainter.on('init', function () {
                $.event.trigger({ type: "newProductsAddedToPageEvent" });
            });

            var isRtl = $("html").attr("dir") == "rtl";

            productsContainter.slick({
                rtl: isRtl,
                infinite: true,
                rows: 2,
                slidesToShow: 4,
                slidesToScroll: 1,
                arrows: true,
                respondTo: 'slider',
                easing: "swing",
                draggable: false,
                pauseOnHover: true,
                responsive: getHomePageCarouselResponsiveSettings()
            });
        }

        function getHomePageCarouselResponsiveSettings() {
            var responsiveBreakpointsObj = {};

            if ($('#home-page-carousel-breakpoints').length == 0|| !$('#home-page-carousel-breakpoints').val()) {
                return responsiveBreakpointsObj;
            }

            try {
                responsiveBreakpointsObj = JSON.parse($('#home-page-carousel-breakpoints').val());
            }
            catch (e) {
                console.log('Invalid home page slider responsive breakpoints setting value! (EmporiumThemeSettings.ResponsiveBreakpointsForHomePageProducts)');
            }

            return responsiveBreakpointsObj;
        }

        //This function is written in a way that allows it to be used in other themes if needed
        function setMaxHeightOnMobile(element, additionalHeight) {

            if (typeof additionalHeight === 'string') {

                additionalHeight = $(additionalHeight).outerHeight();

            } else if (typeof additionalHeight === 'undefined') {

                additionalHeight = 0;

            }

            if ($(window).width() <= 1024) {
                var storeThemeHeight = $('.header-storetheme').length ? $('.header-storetheme').outerHeight() : 0;

                $(element).css('max-height', $(window).height() - additionalHeight - storeThemeHeight);

            }
            else {
                $(element).removeAttr('style');
            }
        }
    });
})(jQuery);