﻿// Copyright 2019 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;

namespace VDrumExplorer.Data.Fields
{
    public sealed class DynamicOverlay : FieldBase, IContainerField
    {
        public ModuleAddress SwitchAddress { get; }
        
        private readonly string? switchTransform;
        internal IReadOnlyList<Container> OverlaidContainers { get; }
        
        // No condition; overlays are already conditional effectively.
        internal DynamicOverlay(FieldBase.Parameters common, ModuleAddress switchAddress, string? switchTransform, IReadOnlyList<Container> containers)
            : base(common) =>
            (SwitchAddress, this.switchTransform, OverlaidContainers) = (switchAddress, switchTransform, containers);

        public IEnumerable<IField> Children(ModuleData data) => GetOverlaidContainer(data).Fields;

        public Container GetOverlaidContainer(ModuleData data)
        {
            var field = Schema.PrimitiveFieldsByAddress[SwitchAddress];
            int index = switchTransform switch
            {
                null => ((NumericFieldBase) field).GetRawValue(data),
                "instrumentGroup" => ((InstrumentField) field).GetInstrument(data).Group.Index,
                _ => throw new InvalidOperationException($"Invalid switch transform '{switchTransform}'")
            };
            return OverlaidContainers[index];
        }
    }
}
