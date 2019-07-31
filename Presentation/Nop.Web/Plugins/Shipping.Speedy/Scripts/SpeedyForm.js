window.SpeedyForm = (function () {

    function SpeedyForm() {
        var self = this;

        $(document).on("change", "input[name='shippingoption']", checkFormVisible);
        var initialized = false;
        var selectedSiteId;
        var deliveryOption = "";
        var officeId = -1;
        var streetName = "";
        var streetNo = "";
        var streetId = 0;
        var quarter = "";
        var quarterId = 0;
        var formIsComplete = false;
        var isSaved = false;
        var T;


        self.init = function () {
            T = window.SpeedyResources.T;

            if (window.ShippingMethod != undefined) { //if onepagecheckout
                //override OnePageCheckout object
                window.ShippingMethod = (function (shippingMethod) {

                    var save = shippingMethod.save;

                    shippingMethod.save = function () {

                        if (!isSaved &&
                            $("input[name='shippingoption']:checked").length > 0 &&
                            $("input[name='shippingoption']:checked").val().indexOf("Shipping.Speedy") >= 0) {
                            var _this = this;
                            saveForm(function () {
                                save.call(_this);
                            });
                        } else {
                            save.call(this);
                        }

                    }
                    return shippingMethod;
                })(window.ShippingMethod);
            } else {
                $(".shipping-method-next-step-button").on("click",
                    function (e) {

                        if (!isSaved &&
                            $("input[name='shippingoption']:checked").length > 0 &&
                            $("input[name='shippingoption']:checked").val().indexOf("Shipping.Speedy") >= 0) {

                            e.preventDefault();
                            e.stopPropagation();

                            saveForm(function () {
                                $(".shipping-method-next-step-button").trigger("click");
                            });
                            return false;
                        }
                    });
            }

            checkFormVisible();

            $(document).on("change",
                "input[name='receivetype']",
                function () {
                    if (selectedSiteId > 0)
                        reloadForm();
                });


            $(document).on("change",
                "#officeId",
                function () {
                    reloadForm();
                });
        }



        function checkFormVisible() {
            console.log("checkVisible");

            if ($("input[name='shippingoption']:checked").val().indexOf("Shipping.Speedy") >= 0) {
                $("#speedy_form").show();
                if (!initialized) {

                    $(".js_siteInpit").autocomplete({
                        source: function (request, response) {
                            $.ajax({
                                url: "/Plugins/SpeedyShipping/GetSites",
                                dataType: "json",
                                data: "term=" + request.term,
                                success: function (data) {
                                    $("#speedy_form").unblock();
                                    response($.map(data.results,
                                        function (item) {
                                            return {
                                                label: item.text,
                                                value: item.id
                                            };
                                        }));
                                },
                                beforeSend: function () {
                                    $("#speedy_form").block({
                                        message: T("Speedy.AddressForm.LoadingFormData")
                                    });
                                }
                            });
                        },
                        minLength: 2,
                        select: function (event, ui) {
                            var siteId = ui.item.value;
                            selectedSiteId = siteId;
                            $(".js_siteInpit").val(ui.item.label);
                            reloadForm();
                            return false;
                        }
                    });

                    initialized = true;
                }
            } else
                $("#speedy_form").hide();
        }

        function saveForm(calbackSuccess) {
            var addressNote = $("input[name='AddressNote']").val(),
                entrance = $("#inputEntrance").val(),
                floor = $("#inputEntrance").val(),
                apNumber = $("#inputApNumber").val(),
                block = $("#inputBlock").val();

            if (validate()) {
                $.ajax({
                    url: "/Plugins/SpeedyShipping/SaveForm",
                    data: {
                        OfficeId: officeId,
                        SiteId: selectedSiteId,
                        DeliveryOption: deliveryOption,
                        StreetName: streetName,
                        StreetNo: streetNo,
                        StreetId: streetId,
                        QuarterId: quarterId,
                        Quarter: quarter,
                        Floor: floor,
                        Block: block,
                        ApNumber: apNumber,
                        Entrance: entrance,
                        AddressNote: addressNote
                    },
                    dataType: 'html',
                    type: 'POST',
                    beforeSend: function () {
                        $("#speedy_form").block({ message: T("Speedy.AddressForm.LoadingFormData") });
                    }
                })
                    .done(function () {
                        isSaved = true;
                        $("#speedy_form").unblock();
                        calbackSuccess();
                    });

                return true;
            }
            return false;
        }

        function reloadForm() {
            var officeId = $("#officeId").val(),
                siteId = selectedSiteId,
                deliveryOption = $("input[name='receivetype']:checked").val();

            isSaved = false;

            $.ajax({
                url: "/Plugins/SpeedyShipping/SpeedyForm",
                data: { OfficeId: officeId, SiteId: siteId, DeliveryOption: deliveryOption },
                dataType: 'html',
                type: 'POST',
                beforeSend: function () {
                    $("#speedy_form").block({ message: T("Speedy.AddressForm.LoadingFormData") });
                }
            })
                .done(function (html) {
                    $("#changableForm").html(html);
                    $("#speedy_form").unblock();
                    initAddressForm();
                    validate();
                });
        }

        function initAddressForm() {
            var $inputStreetName = $("#inputStreetName"),
                $inputQuarter = $("#inputQuarter");

            if ($inputStreetName.length > 0 && $inputQuarter.length > 0) {

                $inputStreetName.autocomplete({
                    source: function (request, response) {
                        $.ajax({
                            url: "/Plugins/SpeedyShipping/GetStreets",
                            dataType: "json",
                            data: "term=" + request.term,
                            success: function (data) {
                                $("#speedy_form").unblock();
                                response($.map(data.results,
                                    function (item) {
                                        return {
                                            label: item.text,
                                            value: item.id
                                        };
                                    }));
                            },
                            beforeSend: function () {
                                $("#speedy_form").block({ message: T("Speedy.AddressForm.LoadingFormData") });
                            }
                        });
                    },
                    minLength: 2,
                    select: function (event, ui) {
                        streetId = ui.item.value;
                        streetName = ui.item.label;
                        $inputStreetName.val(ui.item.label);
                        return false;
                    }
                });

                $inputQuarter.autocomplete({
                    source: function (request, response) {
                        $.ajax({
                            url: "/Plugins/SpeedyShipping/GetQuarters",
                            dataType: "json",
                            data: "term=" + request.term,
                            success: function (data) {
                                $("#speedy_form").unblock();
                                response($.map(data.results,
                                    function (item) {
                                        return {
                                            label: item.text,
                                            value: item.id
                                        };
                                    }));
                            },
                            beforeSend: function () {
                                $("#speedy_form").block({ message: T("Speedy.AddressForm.LoadingFormData") });
                            }
                        });
                    },
                    minLength: 2,
                    select: function (event, ui) {
                        quarterId = ui.item.value;
                        quarter = ui.item.label;
                        $inputQuarter.val(ui.item.label);
                        return false;
                    }
                });

            }

        }

        function validate() {

            /*
             *  Если выбран наш метод, то проверяем:
             *  1. Наличие SiteId
             *  2. В зависимости от выбранного типа доставки проверяем:
             *      2.1 Автомат или Офис
             *          Должен быть выбран офис (OfficeId)
             *      2.2 Доставка на адрес
             *          Должна быть указана улица и номер дома (StreetName, StreetNo)
             * 3. Выбраный метод доставки (ServiceId)
             *    Метод доставки появляется после выбора SiteId и OfficeId. В случае если выбран адрес, то сразу после выбора SiteId
             */
            var result = true;

            deliveryOption = $("input[name='receivetype']:checked").val() || 0;

            if (selectedSiteId == null || selectedSiteId == 0) {
                console.log("Invalid siteId");
                result = false;
            }

            if (deliveryOption == "Address") {

                streetName = $("input[name='StreetName']").val() || "";
                streetNo = $("input[name='StreetNo']").val() || "";

                if (streetName.length < 2) {
                    console.log("Invalid streetName");
                    result = false;
                }
                if (streetNo.length <= 0) {
                    console.log("Invalid streetNo");
                    result = false;
                }

            } else {
                officeId = $("#officeId").val() || 0;

                if (officeId <= 0) {
                    console.log("Invalid officeId");
                    result = false;
                }
            }
            formIsComplete = result;
            return result;
        }

        return self;
    }

    var form = SpeedyForm();

    $(function () {
        form.init();
    });

}());
