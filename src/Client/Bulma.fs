module Bulma
    open Sutil.Attr
    open Sutil.Html

    let createElement el className =
        fun props -> el <| [class' className] @ props

    module Navbar =
        let brand = createElement Html.nav "navbar-brand"
        let item = createElement Html.a "navbar-item"
        let menu = createElement Html.div "navbar-menu"
