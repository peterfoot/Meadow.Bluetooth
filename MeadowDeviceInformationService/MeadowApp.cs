using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Gateways.Bluetooth;
using Meadow.Peripherals.Leds;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MeadowDeviceInformationService
{
    // Change F7CoreComputeV2 to F7FeatherV2 (or F7FeatherV1) for Feather boards
    public class MeadowApp : App<F7CoreComputeV2>
    {
        private IProjectLabHardware? _projectLab;
        private Definition? definition;

        public override Task Run()
        {
            Resolver.Log.Info("Run...");

            Resolver.Log.Info("Hello, Meadow Core-Compute!");

            return base.Run();
        }

        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            _projectLab = ProjectLab.Create();

            try
            {
                definition = new Definition("ProjectLab",
                    new Service("DeviceInformation", 0x180A,
                    new CharacteristicString("ManufacturerName", uuid: "00002a29-0000-1000-8000-00805f9b34fb", //2A29
                        permissions: CharacteristicPermission.Read,
                        properties: CharacteristicProperty.Read, 32),
                    new CharacteristicString("ModelNumber", uuid: "00002a24-0000-1000-8000-00805f9b34fb", //2A24
                        permissions: CharacteristicPermission.Read,
                        properties: CharacteristicProperty.Read, 32),
                    new CharacteristicString("HardwareRevision", uuid: "00002a27-0000-1000-8000-00805f9b34fb", //2A27
                        permissions: CharacteristicPermission.Read,
                        properties: CharacteristicProperty.Read, 32)));

                Device.BluetoothAdapter.StartBluetoothServer(definition);

                // set initial values
                definition.Services[0].Characteristics["ManufacturerName"].SetValue("Wilderness Labs");
                definition.Services[0].Characteristics["ModelNumber"].SetValue("F7CoreComputeV2");
                definition.Services[0].Characteristics["HardwareRevision"].SetValue(_projectLab.RevisionString);

            }
            catch (Exception ex) { }

            return base.Initialize();
        }
    }

    public class CharacteristicBytes : Characteristic<byte[]>
    {
        public CharacteristicBytes(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, params Descriptor[] descriptors) 
            : base(name, uuid, permissions, properties, descriptors) { }
        protected override byte[] SerializeValue(byte[] value)
        {
            return value;
        }

        public override void HandleDataWrite(byte[] data)
        {
            RaiseValueSet(data);
        }
    }
}