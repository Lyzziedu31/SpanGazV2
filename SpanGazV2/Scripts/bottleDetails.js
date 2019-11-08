//(function ($) {
//    function BottleDetails() {
//        var $this = this;
//        function intialize() {
//            $("#FK_ID_order_details").change(function () {
//                var bottleDetails = $("#FK_ID_order_details");
//                bottleDetails.html('');
//                $.get("list",
//                    {
//                        id: $(this).val()
//                    }, function (data) {
//                        _bottleDetails.html(data);
//                    })
//            });
//        }
//        $this.init = function () {
//            intialize();
//        }
//    }
//    $(function () {
//        var self = new BottleDetails();
//        self.init();
//    })
//})(jQuery)  