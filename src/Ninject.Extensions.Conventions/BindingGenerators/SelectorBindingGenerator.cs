﻿// -------------------------------------------------------------------------------------------------
// <copyright file="SelectorBindingGenerator.cs" company="Ninject Project Contributors">
//   Copyright (c) 2009-2017 Ninject Project Contributors. All rights reserved.
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   You may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
// -------------------------------------------------------------------------------------------------

namespace Ninject.Extensions.Conventions.BindingGenerators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ninject.Extensions.Conventions.BindingBuilder;
    using Ninject.Extensions.Conventions.Syntax;
    using Ninject.Syntax;

    /// <summary>
    /// Creates bindings of the selected interfaces and base types of the given type.
    /// </summary>
    public class SelectorBindingGenerator : IBindingGenerator
    {
        private readonly IBindingCreator bindingCreator;

        /// <summary>
        /// Evaluates the base type and interfaces of a type.
        /// </summary>
        private readonly IBindableTypeSelector bindableTypeSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorBindingGenerator"/> class.
        /// </summary>
        /// <param name="bindingCreator">The binding creator.</param>
        /// <param name="selector">The selection delegate.</param>
        /// <param name="bindableTypeSelector">The bindable type selector.</param>
        public SelectorBindingGenerator(
            IBindingCreator bindingCreator,
            ServiceSelector selector,
            IBindableTypeSelector bindableTypeSelector)
        {
            this.Selector = selector;
            this.bindingCreator = bindingCreator;
            this.bindableTypeSelector = bindableTypeSelector;
        }

        /// <summary>
        /// Gets the selection delegate.
        /// </summary>
        /// <value>The selector.</value>
        public ServiceSelector Selector { get; private set; }

        /// <summary>
        /// Creates the bindings for a type.
        /// </summary>
        /// <param name="type">The type for which the bindings are created.</param>
        /// <param name="bindingRoot">The binding root that is used to create the bindings.</param>
        /// <returns>
        /// The syntaxes for the created bindings to configure more options.
        /// </returns>
        public IEnumerable<IBindingWhenInNamedWithOrOnSyntax<object>> CreateBindings(Type type, IBindingRoot bindingRoot)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (type.IsInterface || type.IsAbstract)
            {
                return Enumerable.Empty<IBindingWhenInNamedWithOrOnSyntax<object>>();
            }

            var interfaces = this.bindableTypeSelector.GetBindableInterfaces(type);
            var baseTypes = this.bindableTypeSelector.GetBindableBaseTypes(type);
            var selectedTypes = this.Selector.Invoke(type, interfaces.Union(baseTypes));

            return this.bindingCreator.CreateBindings(bindingRoot, selectedTypes, type);
        }
    }
}