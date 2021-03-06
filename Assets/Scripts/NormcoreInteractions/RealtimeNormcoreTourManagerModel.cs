using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Serialization;

[RealtimeModel]
public partial class RealtimeNormcoreTourManagerModel
{
    [RealtimeProperty(1, true)]
    private RealtimeArray<UserIDModel> _visitorsID;

    [RealtimeProperty(2, true, true)]
    private int _floor;

    [RealtimeProperty(3, true, true)]
    private bool _isTourEnded;

    [RealtimeProperty(4, true, true)]
    private bool _isTeleportEnabled;
}

/* ----- Begin Normal Autogenerated Code ----- */
public partial class RealtimeNormcoreTourManagerModel : RealtimeModel {
    public int floor {
        get {
            return _cache.LookForValueInCache(_floor, entry => entry.floorSet, entry => entry.floor);
        }
        set {
            if (this.floor == value) return;
            _cache.UpdateLocalCache(entry => { entry.floorSet = true; entry.floor = value; return entry; });
            InvalidateReliableLength();
            FireFloorDidChange(value);
        }
    }
    
    public bool isTourEnded {
        get {
            return _cache.LookForValueInCache(_isTourEnded, entry => entry.isTourEndedSet, entry => entry.isTourEnded);
        }
        set {
            if (this.isTourEnded == value) return;
            _cache.UpdateLocalCache(entry => { entry.isTourEndedSet = true; entry.isTourEnded = value; return entry; });
            InvalidateReliableLength();
            FireIsTourEndedDidChange(value);
        }
    }
    
    public bool isTeleportEnabled {
        get {
            return _cache.LookForValueInCache(_isTeleportEnabled, entry => entry.isTeleportEnabledSet, entry => entry.isTeleportEnabled);
        }
        set {
            if (this.isTeleportEnabled == value) return;
            _cache.UpdateLocalCache(entry => { entry.isTeleportEnabledSet = true; entry.isTeleportEnabled = value; return entry; });
            InvalidateReliableLength();
            FireIsTeleportEnabledDidChange(value);
        }
    }
    
    public Normal.Realtime.Serialization.RealtimeArray<UserIDModel> visitorsID {
        get { return _visitorsID; }
    }
    
    public delegate void PropertyChangedHandler<in T>(RealtimeNormcoreTourManagerModel model, T value);
    public event PropertyChangedHandler<int> floorDidChange;
    public event PropertyChangedHandler<bool> isTourEndedDidChange;
    public event PropertyChangedHandler<bool> isTeleportEnabledDidChange;
    
    private struct LocalCacheEntry {
        public bool floorSet;
        public int floor;
        public bool isTourEndedSet;
        public bool isTourEnded;
        public bool isTeleportEnabledSet;
        public bool isTeleportEnabled;
    }
    
    private LocalChangeCache<LocalCacheEntry> _cache = new LocalChangeCache<LocalCacheEntry>();
    
    public enum PropertyID : uint {
        VisitorsID = 1,
        Floor = 2,
        IsTourEnded = 3,
        IsTeleportEnabled = 4,
    }
    
    public RealtimeNormcoreTourManagerModel() : this(null) {
    }
    
    public RealtimeNormcoreTourManagerModel(RealtimeModel parent) : base(null, parent) {
        RealtimeModel[] childModels = new RealtimeModel[1];
        
        _visitorsID = new Normal.Realtime.Serialization.RealtimeArray<UserIDModel>();
        childModels[0] = _visitorsID;
        
        SetChildren(childModels);
    }
    
    protected override void OnParentReplaced(RealtimeModel previousParent, RealtimeModel currentParent) {
        UnsubscribeClearCacheCallback();
    }
    
    private void FireFloorDidChange(int value) {
        try {
            floorDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireIsTourEndedDidChange(bool value) {
        try {
            isTourEndedDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    private void FireIsTeleportEnabledDidChange(bool value) {
        try {
            isTeleportEnabledDidChange?.Invoke(this, value);
        } catch (System.Exception exception) {
            UnityEngine.Debug.LogException(exception);
        }
    }
    
    protected override int WriteLength(StreamContext context) {
        int length = 0;
        if (context.fullModel) {
            FlattenCache();
            length += WriteStream.WriteVarint32Length((uint)PropertyID.Floor, (uint)_floor);
            length += WriteStream.WriteVarint32Length((uint)PropertyID.IsTourEnded, _isTourEnded ? 1u : 0u);
            length += WriteStream.WriteVarint32Length((uint)PropertyID.IsTeleportEnabled, _isTeleportEnabled ? 1u : 0u);
        } else if (context.reliableChannel) {
            LocalCacheEntry entry = _cache.localCache;
            if (entry.floorSet) {
                length += WriteStream.WriteVarint32Length((uint)PropertyID.Floor, (uint)entry.floor);
            }
            if (entry.isTourEndedSet) {
                length += WriteStream.WriteVarint32Length((uint)PropertyID.IsTourEnded, entry.isTourEnded ? 1u : 0u);
            }
            if (entry.isTeleportEnabledSet) {
                length += WriteStream.WriteVarint32Length((uint)PropertyID.IsTeleportEnabled, entry.isTeleportEnabled ? 1u : 0u);
            }
        }
        length += WriteStream.WriteCollectionLength((uint)PropertyID.VisitorsID, _visitorsID, context);
        return length;
    }
    
    protected override void Write(WriteStream stream, StreamContext context) {
        var didWriteProperties = false;
        
        if (context.fullModel) {
            stream.WriteVarint32((uint)PropertyID.Floor, (uint)_floor);
            stream.WriteVarint32((uint)PropertyID.IsTourEnded, _isTourEnded ? 1u : 0u);
            stream.WriteVarint32((uint)PropertyID.IsTeleportEnabled, _isTeleportEnabled ? 1u : 0u);
        } else if (context.reliableChannel) {
            LocalCacheEntry entry = _cache.localCache;
            if (entry.floorSet || entry.isTourEndedSet || entry.isTeleportEnabledSet) {
                _cache.PushLocalCacheToInflight(context.updateID);
                ClearCacheOnStreamCallback(context);
            }
            if (entry.floorSet) {
                stream.WriteVarint32((uint)PropertyID.Floor, (uint)entry.floor);
                didWriteProperties = true;
            }
            if (entry.isTourEndedSet) {
                stream.WriteVarint32((uint)PropertyID.IsTourEnded, entry.isTourEnded ? 1u : 0u);
                didWriteProperties = true;
            }
            if (entry.isTeleportEnabledSet) {
                stream.WriteVarint32((uint)PropertyID.IsTeleportEnabled, entry.isTeleportEnabled ? 1u : 0u);
                didWriteProperties = true;
            }
            
            if (didWriteProperties) InvalidateReliableLength();
        }
        stream.WriteCollection((uint)PropertyID.VisitorsID, _visitorsID, context);
    }
    
    protected override void Read(ReadStream stream, StreamContext context) {
        while (stream.ReadNextPropertyID(out uint propertyID)) {
            switch (propertyID) {
                case (uint)PropertyID.VisitorsID: {
                    stream.ReadCollection(_visitorsID, context);
                    break;
                }
                case (uint)PropertyID.Floor: {
                    int previousValue = _floor;
                    _floor = (int)stream.ReadVarint32();
                    bool floorExistsInChangeCache = _cache.ValueExistsInCache(entry => entry.floorSet);
                    if (!floorExistsInChangeCache && _floor != previousValue) {
                        FireFloorDidChange(_floor);
                    }
                    break;
                }
                case (uint)PropertyID.IsTourEnded: {
                    bool previousValue = _isTourEnded;
                    _isTourEnded = (stream.ReadVarint32() != 0);
                    bool isTourEndedExistsInChangeCache = _cache.ValueExistsInCache(entry => entry.isTourEndedSet);
                    if (!isTourEndedExistsInChangeCache && _isTourEnded != previousValue) {
                        FireIsTourEndedDidChange(_isTourEnded);
                    }
                    break;
                }
                case (uint)PropertyID.IsTeleportEnabled: {
                    bool previousValue = _isTeleportEnabled;
                    _isTeleportEnabled = (stream.ReadVarint32() != 0);
                    bool isTeleportEnabledExistsInChangeCache = _cache.ValueExistsInCache(entry => entry.isTeleportEnabledSet);
                    if (!isTeleportEnabledExistsInChangeCache && _isTeleportEnabled != previousValue) {
                        FireIsTeleportEnabledDidChange(_isTeleportEnabled);
                    }
                    break;
                }
                default: {
                    stream.SkipProperty();
                    break;
                }
            }
        }
    }
    
    #region Cache Operations
    
    private StreamEventDispatcher _streamEventDispatcher;
    
    private void FlattenCache() {
        _floor = floor;
        _isTourEnded = isTourEnded;
        _isTeleportEnabled = isTeleportEnabled;
        _cache.Clear();
    }
    
    private void ClearCache(uint updateID) {
        _cache.RemoveUpdateFromInflight(updateID);
    }
    
    private void ClearCacheOnStreamCallback(StreamContext context) {
        if (_streamEventDispatcher != context.dispatcher) {
            UnsubscribeClearCacheCallback(); // unsub from previous dispatcher
        }
        _streamEventDispatcher = context.dispatcher;
        _streamEventDispatcher.AddStreamCallback(context.updateID, ClearCache);
    }
    
    private void UnsubscribeClearCacheCallback() {
        if (_streamEventDispatcher != null) {
            _streamEventDispatcher.RemoveStreamCallback(ClearCache);
            _streamEventDispatcher = null;
        }
    }
    
    #endregion
}
/* ----- End Normal Autogenerated Code ----- */
