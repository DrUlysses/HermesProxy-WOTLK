using System.Collections.Generic;
using Framework.Constants;
using Framework.GameMath;
using HermesProxy.World.Enums;
using HermesProxy.World.Objects;

namespace HermesProxy.World.Server.Packets;

public class MonsterMove : ServerPacket
{
	public WowGuid128 MoverGUID;

	public ServerSideMovement MoveSpline;

	public List<Vector3> Points = new List<Vector3>();

	public List<Vector3> PackedDeltas = new List<Vector3>();

	public MonsterMove(WowGuid128 guid, ServerSideMovement moveSpline)
		: base(Opcode.SMSG_ON_MONSTER_MOVE, ConnectionType.Instance)
	{
		if (moveSpline.SplineFlags.HasFlag(SplineFlagModern.UncompressedPath))
		{
			if (!moveSpline.SplineFlags.HasFlag(SplineFlagModern.Cyclic))
			{
				foreach (Vector3 point in moveSpline.SplinePoints)
				{
					this.Points.Add(point);
				}
				if (moveSpline.EndPosition != Vector3.Zero)
				{
					this.Points.Add(moveSpline.EndPosition);
				}
			}
			else
			{
				if (moveSpline.EndPosition != Vector3.Zero)
				{
					this.Points.Add(moveSpline.EndPosition);
				}
				foreach (Vector3 point2 in moveSpline.SplinePoints)
				{
					this.Points.Add(point2);
				}
			}
		}
		else if (moveSpline.EndPosition != Vector3.Zero)
		{
			this.Points.Add(moveSpline.EndPosition);
			if (moveSpline.SplinePoints.Count > 0)
			{
				Vector3 middle = (moveSpline.StartPosition + moveSpline.EndPosition) / 2f;
				for (int i = 0; i < moveSpline.SplinePoints.Count; i++)
				{
					this.PackedDeltas.Add(middle - moveSpline.SplinePoints[i]);
				}
			}
		}
		this.MoverGUID = guid;
		this.MoveSpline = moveSpline;
	}

	protected override void Write()
	{
		base._worldPacket.WritePackedGuid128(this.MoverGUID);
		base._worldPacket.WriteVector3(this.MoveSpline.StartPosition);
		base._worldPacket.WriteUInt32(this.MoveSpline.SplineId);
		base._worldPacket.WriteVector3(Vector3.Zero);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBits((this.Points.Count == 0) ? 2 : 0, 3);
		base._worldPacket.WriteUInt32((uint)this.MoveSpline.SplineFlags);
		base._worldPacket.WriteInt32(0);
		base._worldPacket.WriteUInt32(this.MoveSpline.SplineTimeFull);
		base._worldPacket.WriteUInt32(0u);
		base._worldPacket.WriteUInt8(this.MoveSpline.SplineMode);
		base._worldPacket.WritePackedGuid128((this.MoveSpline.TransportGuid != null) ? this.MoveSpline.TransportGuid : WowGuid128.Empty);
		base._worldPacket.WriteInt8(this.MoveSpline.TransportSeat);
		base._worldPacket.WriteBits((byte)this.MoveSpline.SplineType, 2);
		base._worldPacket.WriteBits(this.Points.Count, 16);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBits(this.PackedDeltas.Count, 16);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.WriteBit(bit: false);
		base._worldPacket.FlushBits();
		switch (this.MoveSpline.SplineType)
		{
		case SplineTypeModern.FacingSpot:
			base._worldPacket.WriteVector3(this.MoveSpline.FinalFacingSpot);
			break;
		case SplineTypeModern.FacingTarget:
			base._worldPacket.WriteFloat(this.MoveSpline.FinalOrientation);
			base._worldPacket.WritePackedGuid128(this.MoveSpline.FinalFacingGuid);
			break;
		case SplineTypeModern.FacingAngle:
			base._worldPacket.WriteFloat(this.MoveSpline.FinalOrientation);
			break;
		}
		foreach (Vector3 pos in this.Points)
		{
			base._worldPacket.WriteVector3(pos);
		}
		foreach (Vector3 pos2 in this.PackedDeltas)
		{
			base._worldPacket.WritePackXYZ(pos2);
		}
	}
}
